using UnityEngine;

namespace LanguageTutor.Data
{
    /// <summary>
    /// Configuration for Speech-to-Text service.
    /// Create via: Assets -> Create -> Language Tutor -> STT Config
    /// </summary>
    [CreateAssetMenu(fileName = "STTConfig", menuName = "Language Tutor/STT Config", order = 3)]
    public class STTConfig : ScriptableObject
    {
        [Header("Service Configuration")]
        [Tooltip("Speech-to-text provider (Whisper, Azure, Google, etc.)")]
        public STTProvider provider = STTProvider.Whisper;

        [Tooltip("Default language code for transcription (e.g., 'en', 'de', 'es')")]
        public string defaultLanguage = "en";

        [Header("Recording Settings")]
        [Tooltip("Maximum recording duration in seconds")]
        [Range(5, 120)]
        public int maxRecordingDuration = 30;

        [Tooltip("Microphone sample rate (Hz)")]
        public int sampleRate = 44100;

        [Tooltip("Trim silence threshold (0.0 - 1.0)")]
        [Range(0.0f, 1.0f)]
        public float silenceThreshold = 0.01f;

        [Tooltip("Minimum audio length in samples before processing")]
        [Range(100, 10000)]
        public int minAudioLength = 1000;

        [Header("Processing")]
        [Tooltip("Enable voice activity detection to auto-stop recording")]
        public bool enableVAD = false;

        [Tooltip("Silence duration (seconds) to auto-stop recording")]
        [Range(0.5f, 5.0f)]
        public float vadSilenceDuration = 1.5f;

        [Tooltip("Enable pronunciation assessment with confidence scores")]
        public bool enablePronunciationAssessment = false;

        [Header("Performance")]
        [Tooltip("Request timeout in seconds")]
        [Range(5, 60)]
        public int timeoutSeconds = 30;

        [Tooltip("Number of retry attempts on failure")]
        [Range(0, 5)]
        public int maxRetries = 2;

        [Header("Whisper-Specific Settings")]
        [Tooltip("Whisper model size (tiny, base, small, medium, large)")]
        public string whisperModelSize = "base";

        [Tooltip("Enable translation to English")]
        public bool whisperTranslateToEnglish = false;
    }

    public enum STTProvider
    {
        Whisper,
        Azure,
        Google,
        AWS
    }
}
