using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using LanguageTutor.Data;

namespace LanguageTutor.Services
{
    /// <summary>
    /// AllTalk TTS service implementation.
    /// Communicates with local AllTalk TTS server for speech synthesis.
    /// </summary>
    public class AllTalkService : ITTSService
    {
        private readonly TTSConfig _config;
        private readonly MonoBehaviour _coroutineRunner;
        private readonly Dictionary<string, AudioClip> _audioCache;
        private UnityWebRequest _currentRequest;

        public AllTalkService(TTSConfig config, MonoBehaviour coroutineRunner)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _coroutineRunner = coroutineRunner ?? throw new ArgumentNullException(nameof(coroutineRunner));
            _audioCache = new Dictionary<string, AudioClip>();
        }

        public async Task<AudioClip> SynthesizeSpeechAsync(string text, string voiceName = null, string language = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            // Check cache
            string cacheKey = GetCacheKey(text, voiceName, language);
            if (_config.enableCaching && _audioCache.TryGetValue(cacheKey, out AudioClip cachedClip))
            {
                Debug.Log($"[AllTalkService] Using cached audio for: {text.Substring(0, Math.Min(30, text.Length))}...");
                return cachedClip;
            }

            var tcs = new TaskCompletionSource<AudioClip>();
            _coroutineRunner.StartCoroutine(GenerateAudioCoroutine(text, voiceName, language, cacheKey, tcs));
            return await tcs.Task;
        }

        public async Task<string[]> GetAvailableVoicesAsync()
        {
            // AllTalk typically has voices in a specific directory
            // This is a simplified implementation - extend based on AllTalk API
            return await Task.FromResult(new[] { _config.defaultVoice });
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                // Try a simple health check - synthesize a very short audio
                var testClip = await SynthesizeSpeechAsync("test", null, null);
                return testClip != null;
            }
            catch
            {
                return false;
            }
        }

        public void CancelSynthesis()
        {
            if (_currentRequest != null && !_currentRequest.isDone)
            {
                _currentRequest.Abort();
                _currentRequest = null;
                Debug.Log("[AllTalkService] Synthesis cancelled");
            }
        }

        private System.Collections.IEnumerator GenerateAudioCoroutine(
            string text, 
            string voiceName, 
            string language,
            string cacheKey,
            TaskCompletionSource<AudioClip> tcs)
        {
            WWWForm form = new WWWForm();
            form.AddField("text_input", text);
            form.AddField("character_voice", voiceName ?? _config.defaultVoice);
            form.AddField("language", language ?? _config.defaultLanguage);
            form.AddField("text_filtering", "standard");
            form.AddField("autoplay", "false");
            form.AddField("narrator_enabled", "false");

            _currentRequest = UnityWebRequest.Post(_config.GetFullUrl(), form);
            _currentRequest.timeout = _config.timeoutSeconds;

            yield return _currentRequest.SendWebRequest();

            if (_currentRequest.result != UnityWebRequest.Result.Success)
            {
                tcs.SetException(new Exception($"AllTalk request failed: {_currentRequest.error}"));
                _currentRequest = null;
                yield break;
            }

            string responseText = _currentRequest.downloadHandler.text;
            
            Debug.Log($"[AllTalkService] Response: {responseText}");
            
            AllTalkResponse response = null;
            
            // Parse response
            response = JsonUtility.FromJson<AllTalkResponse>(responseText);
            
            if (response == null || string.IsNullOrEmpty(response.output_file_path))
            {
                tcs.SetException(new Exception("AllTalk returned empty file path"));
                _currentRequest = null;
                yield break;
            }

            Debug.Log($"[AllTalkService] Loading audio from disk: {response.output_file_path}");
            
            // Load the audio file (use file:// not file:///)
            string audioUrl = "file://" + response.output_file_path;
            
            using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
            {
                yield return audioRequest.SendWebRequest();

                if (audioRequest.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                    
                    if (clip != null)
                    {
                        // Cache the clip
                        if (_config.enableCaching)
                        {
                            CacheAudioClip(cacheKey, clip);
                        }
                        
                        tcs.SetResult(clip);
                    }
                    else
                    {
                        tcs.SetException(new Exception("Failed to create AudioClip from WAV data"));
                    }
                }
                else
                {
                    tcs.SetException(new Exception($"Failed to load audio file: {audioRequest.error}"));
                }
            }

            _currentRequest = null;
        }

        private string GetCacheKey(string text, string voice, string language)
        {
            return $"{text}_{voice ?? _config.defaultVoice}_{language ?? _config.defaultLanguage}";
        }

        private void CacheAudioClip(string key, AudioClip clip)
        {
            // Manage cache size
            if (_audioCache.Count >= _config.maxCacheSize)
            {
                // Remove oldest entry (simple FIFO strategy)
                var enumerator = _audioCache.GetEnumerator();
                enumerator.MoveNext();
                string oldestKey = enumerator.Current.Key;
                _audioCache.Remove(oldestKey);
            }

            _audioCache[key] = clip;
        }

        #region DTOs
        [Serializable]
        private class AllTalkResponse
        {
            public string output_file_path;
            public string output_file_url;
            public string output_cache_url;
        }
        #endregion
    }
}
