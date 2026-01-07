using UnityEngine;

namespace LanguageTutor.Data
{
    /// <summary>
    /// Configuration for conversation and language learning behavior.
    /// Create via: Assets -> Create -> Language Tutor -> Conversation Config
    /// </summary>
    [CreateAssetMenu(fileName = "ConversationConfig", menuName = "Language Tutor/Conversation Config", order = 4)]
    public class ConversationConfig : ScriptableObject
    {
        [Header("Conversation Management")]
        [Tooltip("Maximum number of messages to retain in conversation history")]
        [Range(5, 50)]
        public int maxHistoryLength = 20;

        [Tooltip("Include system messages in conversation context")]
        public bool includeSystemMessagesInHistory = false;

        [Tooltip("Auto-summarize conversation history when it exceeds max length")]
        public bool autoSummarizeHistory = true;

        [Header("Language Learning")]
        [Tooltip("Target language for learning (e.g., 'en', 'de', 'es', 'fr')")]
        public string targetLanguage = "en";

        [Tooltip("User's proficiency level (CEFR scale)")]
        public LanguageLevel userLevel = LanguageLevel.A2_Elementary;

        [Tooltip("Enable automatic grammar correction feedback")]
        public bool enableGrammarCorrection = true;

        [Tooltip("Enable vocabulary suggestions during conversation")]
        public bool enableVocabularySuggestions = true;

        [Tooltip("Pronunciation feedback threshold (0.0 - 1.0, lower = stricter)")]
        [Range(0.5f, 0.95f)]
        public float pronunciationThreshold = 0.75f;

        [Header("Interaction Behavior")]
        [Tooltip("NPC response delay to simulate thinking (seconds)")]
        [Range(0.0f, 3.0f)]
        public float responseDelay = 0.5f;

        [Tooltip("Show typing indicator while generating response")]
        public bool showTypingIndicator = true;

        [Tooltip("Display subtitles/transcriptions")]
        public bool showSubtitles = true;

        [Tooltip("Auto-play TTS audio when response is ready")]
        public bool autoPlayTTS = true;

        [Header("Tutorial Mode")]
        [Tooltip("Enable guided conversation scenarios")]
        public bool enableScenarios = false;

        [Tooltip("Provide hints when user is stuck")]
        public bool provideHints = true;

        [Tooltip("Time before showing hint (seconds)")]
        [Range(5.0f, 30.0f)]
        public float hintDelaySeconds = 10.0f;

        [Header("Feedback")]
        [Tooltip("Enable positive reinforcement for good pronunciation")]
        public bool enableEncouragement = true;

        [Tooltip("Show detailed error explanations")]
        public bool showDetailedErrors = true;
    }

    /// <summary>
    /// CEFR (Common European Framework of Reference) language proficiency levels
    /// </summary>
    public enum LanguageLevel
    {
        A1_Beginner,
        A2_Elementary,
        B1_Intermediate,
        B2_UpperIntermediate,
        C1_Advanced,
        C2_Proficient
    }
}
