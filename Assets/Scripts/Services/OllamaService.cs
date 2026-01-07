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
    /// Ollama LLM service implementation.
    /// Communicates with local Ollama server for AI text generation.
    /// </summary>
    public class OllamaService : ILLMService
    {
        private readonly LLMConfig _config;
        private MonoBehaviour _coroutineRunner;

        public OllamaService(LLMConfig config, MonoBehaviour coroutineRunner)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
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

            var tcs = new TaskCompletionSource<string>();

            _coroutineRunner.StartCoroutine(SendRequestCoroutine(prompt, systemPrompt, conversationHistory, tcs));

            return await tcs.Task;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var testResponse = await GenerateResponseAsync("test", "Reply with 'ok'", null);
                return !string.IsNullOrEmpty(testResponse);
            }
            catch
            {
                return false;
            }
        }

        private System.Collections.IEnumerator SendRequestCoroutine(
            string prompt, 
            string systemPrompt, 
            List<ConversationMessage> conversationHistory,
            TaskCompletionSource<string> tcs)
        {
            // Build the complete prompt with system message and history
            string fullPrompt = BuildFullPrompt(prompt, systemPrompt, conversationHistory);

            var request = new OllamaRequest
            {
                model = _config.modelName,
                prompt = fullPrompt,
                stream = _config.enableStreaming,
                options = new OllamaOptions
                {
                    temperature = _config.temperature,
                    num_predict = _config.maxTokens
                }
            };

            string json = JsonUtility.ToJson(request);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest webRequest = new UnityWebRequest(_config.GetFullUrl(), "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.timeout = _config.timeoutSeconds;

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonUtility.FromJson<OllamaResponse>(webRequest.downloadHandler.text);
                        
                        if (string.IsNullOrEmpty(response.response))
                        {
                            tcs.SetException(new Exception("Empty response from Ollama service"));
                        }
                        else
                        {
                            tcs.SetResult(response.response);
                        }
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(new Exception($"Failed to parse Ollama response: {ex.Message}"));
                    }
                }
                else
                {
                    string errorMsg = $"Ollama request failed: {webRequest.error}";
                    if (webRequest.responseCode == 404)
                        errorMsg += " - Model not found. Run 'ollama pull " + _config.modelName + "'";
                    
                    tcs.SetException(new Exception(errorMsg));
                }
            }
        }

        private string BuildFullPrompt(string userPrompt, string systemPrompt, List<ConversationMessage> history)
        {
            var sb = new StringBuilder();

            // Add system prompt
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                sb.AppendLine(systemPrompt);
                sb.AppendLine();
            }

            // Add conversation history (if any)
            if (history != null && history.Count > 0)
            {
                sb.AppendLine("Previous conversation:");
                foreach (var msg in history)
                {
                    string role = msg.Role == MessageRole.User ? "User" : "Assistant";
                    sb.AppendLine($"{role}: {msg.Content}");
                }
                sb.AppendLine();
            }

            // Add current user prompt
            sb.Append("User says: ");
            sb.Append(userPrompt);

            return sb.ToString();
        }

        #region DTOs
        [Serializable]
        private class OllamaRequest
        {
            public string model;
            public string prompt;
            public bool stream;
            public OllamaOptions options;
        }

        [Serializable]
        private class OllamaOptions
        {
            public float temperature;
            public int num_predict;
        }

        [Serializable]
        private class OllamaResponse
        {
            public string response;
            public string model;
            public bool done;
        }
        #endregion
    }
}
