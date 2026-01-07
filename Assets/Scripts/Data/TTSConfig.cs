using UnityEngine;

namespace LanguageTutor.Data
{
    /// <summary>
    /// Configuration for Text-to-Speech service.
    /// Create via: Assets -> Create -> Language Tutor -> TTS Config
    /// </summary>
    [CreateAssetMenu(fileName = "TTSConfig", menuName = "Language Tutor/TTS Config", order = 2)]
    public class TTSConfig : ScriptableObject
    {
        [Header("Service Configuration")]
        [Tooltip("Base URL for the TTS service (e.g., http://127.0.0.1:7851)")]
        public string serviceUrl = "http://127.0.0.1:7851";

        [Tooltip("API endpoint path (e.g., /api/tts-generate for AllTalk)")]
        public string endpointPath = "/api/tts-generate";

        [Header("Voice Settings")]
        [Tooltip("Default voice to use for speech synthesis")]
        public string defaultVoice = "male_01.wav";

        [Tooltip("Default language code (e.g., 'en', 'de', 'es', 'fr')")]
        public string defaultLanguage = "en";

        [Tooltip("Speech rate/speed (0.5 = slow, 1.0 = normal, 2.0 = fast)")]
        [Range(0.5f, 2.0f)]
        public float speechRate = 1.0f;

        [Tooltip("Voice pitch adjustment")]
        [Range(0.5f, 2.0f)]
        public float pitch = 1.0f;

        [Header("Audio Quality")]
        [Tooltip("Sample rate for generated audio (Hz)")]
        public int sampleRate = 22050;

        [Tooltip("Audio output format (wav, mp3, ogg)")]
        public AudioFormat outputFormat = AudioFormat.WAV;

        [Header("Performance")]
        [Tooltip("Request timeout in seconds")]
        [Range(5, 60)]
        public int timeoutSeconds = 20;

        [Tooltip("Number of retry attempts on failure")]
        [Range(0, 5)]
        public int maxRetries = 2;

        [Tooltip("Cache generated audio clips to avoid re-generation")]
        public bool enableCaching = true;

        [Tooltip("Maximum number of cached audio clips")]
        [Range(10, 100)]
        public int maxCacheSize = 50;

        [Header("Advanced")]
        [Tooltip("Split long texts into smaller chunks for faster generation")]
        public bool enableTextChunking = true;

        [Tooltip("Maximum characters per chunk")]
        [Range(50, 500)]
        public int maxChunkLength = 200;

        /// <summary>
        /// Get the full service URL (base + endpoint)
        /// </summary>
        public string GetFullUrl()
        {
            return serviceUrl.TrimEnd('/') + "/" + endpointPath.TrimStart('/');
        }
    }

    public enum AudioFormat
    {
        WAV,
        MP3,
        OGG
    }
}
