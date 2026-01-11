using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using LanguageTutor.Data;

namespace LanguageTutor.Services
{
    /// <summary>
    /// HuggingFace Inference API service implementation.
    /// Communicates with HuggingFace's OpenAI-compatible router API for AI text generation.
    /// Supports models like google/gemma-3-27b-it, meta-llama/Llama-3-70b-chat-hf, etc.
    /// </summary>
    public class HuggingFaceService : ILLMService
    {
        private readonly LLMConfig _config;
        private MonoBehaviour _coroutineRunner;
        private const string HUGGINGFACE_API_BASE = "https://router.huggingface.co/v1/chat/completions";

        public HuggingFaceService(LLMConfig config, MonoBehaviour coroutineRunner)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));

            if (string.IsNullOrWhiteSpace(_config.apiKey))
            {
                Debug.LogWarning("[HuggingFaceService] API key (HF_TOKEN) is not set. Please add your HuggingFace token in the LLMConfig.");
            }
        }

        public string GetModelName() => _config.modelName;

        public async Task<string> GenerateResponseAsync(string prompt, List<ConversationMessage> conversationHistory = null)
        {
            return await GenerateResponseAsync(prompt, _config.defaultSystemPrompt, conversationHistory);
        }

        public async Task<string> GenerateResponseAsync(string prompt, string systemPrompt, List<ConversationMessage> conversationHistory = null)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

            if (string.IsNullOrWhiteSpace(_config.apiKey))
                throw new InvalidOperationException("API key (HF_TOKEN) is required for HuggingFace service. Please set it in the LLMConfig.");

            var tcs = new TaskCompletionSource<string>();

            _coroutineRunner.StartCoroutine(SendRequestCoroutine(prompt, systemPrompt, conversationHistory, tcs));

            return await tcs.Task;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_config.apiKey))
                    return false;

                var testResponse = await GenerateResponseAsync("test", "Reply with 'ok'", null);
                return !string.IsNullOrEmpty(testResponse);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[HuggingFaceService] Availability check failed: {ex.Message}");
                return false;
            }
        }

        private System.Collections.IEnumerator SendRequestCoroutine(
            string prompt,
            string systemPrompt,
            List<ConversationMessage> conversationHistory,
            TaskCompletionSource<string> tcs)
        {
            // Build messages array with system prompt and conversation history
            var messages = BuildMessages(prompt, systemPrompt, conversationHistory);

            var request = new HuggingFaceRequest
            {
                model = string.IsNullOrEmpty(_config.modelName) ? "google/gemma-3-27b-it" : _config.modelName,
                messages = messages,
                temperature = _config.temperature,
                max_tokens = _config.maxTokens
            };

            string json = JsonUtility.ToJson(request);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            Debug.Log($"[HuggingFaceService] Sending request to HuggingFace API");
            Debug.Log($"[HuggingFaceService] Model: {request.model}");
            Debug.Log($"[HuggingFaceService] Prompt length: {prompt.Length} chars");

            using (UnityWebRequest webRequest = new UnityWebRequest(HUGGINGFACE_API_BASE, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {_config.apiKey}");
                webRequest.timeout = _config.timeoutSeconds;

                float startTime = Time.time;
                var operation = webRequest.SendWebRequest();

                // Manual timeout with progress tracking
                while (!operation.isDone)
                {
                    float elapsed = Time.time - startTime;
                    if (elapsed > _config.timeoutSeconds)
                    {
                        Debug.LogError($"[HuggingFaceService] Request timed out after {elapsed}s");
                        webRequest.Abort();
                        tcs.SetException(new System.Exception($"Request timed out after {elapsed}s"));
                        yield break;
                    }

                    if (elapsed > 5f && Mathf.FloorToInt(elapsed) % 5 == 0)
                    {
                        Debug.Log($"[HuggingFaceService] Still waiting... {elapsed:F1}s elapsed");
                    }

                    yield return null;
                }

                Debug.Log($"[HuggingFaceService] Request completed in {Time.time - startTime:F2}s");
                Debug.Log($"[HuggingFaceService] Response Code: {webRequest.responseCode}");

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonUtility.FromJson<HuggingFaceResponse>(webRequest.downloadHandler.text);

                        if (response.choices == null || response.choices.Length == 0)
                        {
                            Debug.LogError("[HuggingFaceService] No choices in response");
                            tcs.SetException(new Exception("No choices in HuggingFace response"));
                            yield break;
                        }

                        var choice = response.choices[0];
                        if (choice.message == null || string.IsNullOrEmpty(choice.message.content))
                        {
                            Debug.LogError("[HuggingFaceService] No message content in response");
                            tcs.SetException(new Exception("No message content in HuggingFace response"));
                            yield break;
                        }

                        string responseText = choice.message.content;
                        Debug.Log($"[HuggingFaceService] Response received: {responseText.Substring(0, Math.Min(100, responseText.Length))}...");
                        tcs.SetResult(responseText);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[HuggingFaceService] Failed to parse response: {ex.Message}");
                        Debug.LogError($"[HuggingFaceService] Response body: {webRequest.downloadHandler.text}");
                        tcs.SetException(new Exception($"Failed to parse HuggingFace response: {ex.Message}"));
                    }
                }
                else
                {
                    string errorMsg = $"HuggingFace API request failed: {webRequest.error}";
                    if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        errorMsg += $"\nResponse: {webRequest.downloadHandler.text}";
                    }
                    Debug.LogError($"[HuggingFaceService] {errorMsg}");
                    tcs.SetException(new Exception(errorMsg));
                }
            }
        }

        private HuggingFaceMessage[] BuildMessages(string prompt, string systemPrompt, List<ConversationMessage> conversationHistory)
        {
            var messagesList = new List<HuggingFaceMessage>();

            // Build a list of raw messages first (user/assistant only, no system role)
            // Many HuggingFace models require strict alternating user/assistant/user/assistant...
            
            // Collect all user/assistant messages from history
            if (conversationHistory != null)
            {
                foreach (var message in conversationHistory)
                {
                    if (message.Role == MessageRole.System)
                        continue; // Skip system messages, we'll merge system prompt into first user message

                    string role = message.Role == MessageRole.User ? "user" : "assistant";
                    messagesList.Add(new HuggingFaceMessage
                    {
                        role = role,
                        content = message.Content
                    });
                }
            }

            // Add current prompt as user message
            messagesList.Add(new HuggingFaceMessage
            {
                role = "user",
                content = prompt
            });

            // Merge consecutive messages with the same role to ensure alternation
            messagesList = MergeConsecutiveSameRoleMessages(messagesList);

            // Ensure conversation starts with user (required by most HuggingFace models)
            // If first message is assistant, prepend an empty user message
            if (messagesList.Count > 0 && messagesList[0].role == "assistant")
            {
                messagesList.Insert(0, new HuggingFaceMessage
                {
                    role = "user",
                    content = "[Starting conversation]"
                });
            }

            // Prepend system prompt to the first user message (since system role is not supported)
            if (!string.IsNullOrEmpty(systemPrompt) && messagesList.Count > 0)
            {
                for (int i = 0; i < messagesList.Count; i++)
                {
                    if (messagesList[i].role == "user")
                    {
                        messagesList[i].content = $"[Instructions: {systemPrompt}]\n\n{messagesList[i].content}";
                        break;
                    }
                }
            }

            return messagesList.ToArray();
        }

        /// <summary>
        /// Merges consecutive messages with the same role to ensure proper alternation.
        /// HuggingFace models require strict user/assistant/user/assistant pattern.
        /// </summary>
        private List<HuggingFaceMessage> MergeConsecutiveSameRoleMessages(List<HuggingFaceMessage> messages)
        {
            if (messages == null || messages.Count <= 1)
                return messages;

            var merged = new List<HuggingFaceMessage>();
            HuggingFaceMessage current = null;

            foreach (var message in messages)
            {
                if (current == null)
                {
                    current = new HuggingFaceMessage
                    {
                        role = message.role,
                        content = message.content
                    };
                }
                else if (current.role == message.role)
                {
                    // Same role - merge content
                    current.content += "\n\n" + message.content;
                }
                else
                {
                    // Different role - save current and start new
                    merged.Add(current);
                    current = new HuggingFaceMessage
                    {
                        role = message.role,
                        content = message.content
                    };
                }
            }

            // Don't forget the last message
            if (current != null)
            {
                merged.Add(current);
            }

            return merged;
        }
    }

    #region HuggingFace API Data Structures (OpenAI-compatible format)

    [Serializable]
    public class HuggingFaceRequest
    {
        public string model;
        public HuggingFaceMessage[] messages;
        public float temperature;
        public int max_tokens;
    }

    [Serializable]
    public class HuggingFaceMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class HuggingFaceResponse
    {
        public string id;
        public string @object;
        public long created;
        public string model;
        public HuggingFaceChoice[] choices;
        public HuggingFaceUsage usage;
    }

    [Serializable]
    public class HuggingFaceChoice
    {
        public int index;
        public HuggingFaceMessage message;
        public string finish_reason;
    }

    [Serializable]
    public class HuggingFaceUsage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    #endregion
}
