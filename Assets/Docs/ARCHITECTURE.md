# AR Language Tutor - Refactored Architecture

## Overview

This project has been refactored from a prototype into a maintainable, scalable architecture for an AR-based language learning application powered by LLM (Large Language Model) technology.

## Architecture Pattern

The codebase follows a **Service-Oriented Architecture** with **MVC (Model-View-Controller)** pattern and **Command Pattern** for extensible LLM actions.

### Key Principles

- **Separation of Concerns**: Each class has a single, well-defined responsibility
- **Dependency Injection**: Services are injected via constructors/configuration
- **Interface-Based Design**: All services implement interfaces for swappability
- **Configuration over Hard-coding**: ScriptableObjects for all configuration
- **Event-Driven Communication**: Loose coupling through events

---

## Project Structure

```
Assets/_Project/Scripts/
├── Core/                          # Core application logic
│   ├── NPCController.cs           # Main orchestrator (entry point)
│   ├── ConversationPipeline.cs    # STT → LLM → TTS pipeline
│   ├── AudioInputController.cs    # Microphone recording management
│   └── ConversationHistory.cs     # Multi-turn conversation context
│
├── Services/                      # Service layer with interfaces
│   ├── ILLMService.cs            # Interface for LLM providers
│   ├── ITTSService.cs            # Interface for TTS providers
│   ├── ISTTService.cs            # Interface for STT providers
│   ├── OllamaService.cs          # Ollama/Llama3 implementation
│   ├── AllTalkService.cs         # AllTalk TTS implementation
│   └── WhisperService.cs         # Whisper STT implementation
│
├── Actions/                       # LLM Action system (Command Pattern)
│   ├── ILLMAction.cs             # Base action interface
│   ├── LLMActionExecutor.cs      # Action orchestrator with retry logic
│   ├── ChatAction.cs             # Basic conversation
│   ├── GrammarCheckAction.cs     # Grammar correction
│   ├── VocabularyTeachAction.cs  # Vocabulary teaching
│   └── ConversationPracticeAction.cs  # Scenario-based practice
│
├── Data/                          # Configuration ScriptableObjects
│   ├── LLMConfig.cs              # LLM service configuration
│   ├── TTSConfig.cs              # TTS service configuration
│   ├── STTConfig.cs              # STT service configuration
│   └── ConversationConfig.cs     # Conversation behavior settings
│
├── UI/                            # User interface components
│   └── NPCView.cs                # NPC visual presentation layer
│
└── Utilities/                     # Helper utilities
    └── WavUtility.cs             # WAV audio file processing
```

---

## Component Responsibilities

### Core Layer

#### NPCController
**Purpose**: Main entry point that wires together all components  
**Responsibilities**:
- Initialize services and systems
- Coordinate between AudioInput, Pipeline, and View
- Handle button interactions
- Manage conversation state

**Key Methods**:
- `SetActionMode(ActionMode)` - Switch between Chat, GrammarCheck, VocabularyTeach, ConversationPractice
- `ResetConversation()` - Clear conversation history
- `GetConversationSummary()` - Get stats about current session

#### ConversationPipeline
**Purpose**: Orchestrate the complete conversation flow  
**Responsibilities**:
- Execute STT → LLM → TTS pipeline
- Manage conversation history
- Emit events at each stage
- Handle errors and retries

**Pipeline Stages**:
1. **Transcribing**: Convert speech to text (Whisper)
2. **GeneratingResponse**: Get LLM response (Ollama)
3. **SynthesizingSpeech**: Generate audio (AllTalk)
4. **Complete**: All stages finished successfully

#### AudioInputController
**Purpose**: Manage microphone recording  
**Responsibilities**:
- Start/stop recording
- Trim audio to actual length
- Detect insufficient audio
- Emit recording events

#### ConversationHistory
**Purpose**: Maintain multi-turn dialogue context  
**Responsibilities**:
- Store user and assistant messages
- Auto-trim when exceeding max length
- Provide recent messages for context
- Export/import for persistence

---

### Services Layer

#### ILLMService (Interface)
**Implementations**: `OllamaService`  
**Purpose**: Generate text responses using AI models  

**Key Methods**:
```csharp
Task<string> GenerateResponseAsync(string prompt, List<ConversationMessage> history)
Task<bool> IsAvailableAsync()
string GetModelName()
```

**OllamaService Features**:
- Supports conversation history
- Configurable temperature and max tokens
- Automatic retry on failure
- Detailed error messages

#### ITTSService (Interface)
**Implementations**: `AllTalkService`  
**Purpose**: Convert text to speech audio  

**Key Methods**:
```csharp
Task<AudioClip> SynthesizeSpeechAsync(string text, string voice, string language)
Task<string[]> GetAvailableVoicesAsync()
void CancelSynthesis()
```

**AllTalkService Features**:
- Audio caching for repeated phrases
- Multiple voice support
- Configurable speech rate and pitch
- File-based audio loading

#### ISTTService (Interface)
**Implementations**: `WhisperService`  
**Purpose**: Transcribe speech to text  

**Key Methods**:
```csharp
Task<string> TranscribeAsync(AudioClip audio, string language)
Task<TranscriptionResult> TranscribeWithConfidenceAsync(AudioClip audio, string expectedText)
void CancelTranscription()
```

**WhisperService Features**:
- Pronunciation confidence scoring
- Text similarity calculation (Levenshtein distance)
- Multi-language support
- Cancellable transcription

---

### Actions Layer (Generic LLM Action System)

#### Command Pattern Architecture

The action system uses the **Command Pattern** to encapsulate LLM requests as objects. This enables:
- **Extensibility**: Add new actions without modifying existing code
- **Reusability**: Actions can be reused across different contexts
- **Composition**: Chain multiple actions together
- **Testing**: Mock actions for unit testing

#### ILLMAction Interface
```csharp
public interface ILLMAction
{
    Task<LLMActionResult> ExecuteAsync(ILLMService llm, LLMActionContext context);
    string GetActionName();
    bool CanExecute(LLMActionContext context);
}
```

#### LLMActionExecutor
**Purpose**: Execute actions with automatic retry and error handling  

**Features**:
- Configurable retry count and delay
- Execution time tracking
- Sequence execution (chain multiple actions)
- Service availability checking

#### Built-in Actions

**ChatAction**  
Basic conversational responses with system prompt customization.

**GrammarCheckAction**  
Analyzes user input for grammatical errors and provides corrections with explanations.

**VocabularyTeachAction**  
Teaches new words with definitions, examples, and memory tips.

**ConversationPracticeAction**  
Simulates real-world scenarios (ordering food, asking directions, etc.).

#### Creating Custom Actions

```csharp
public class PronunciationFeedbackAction : ILLMAction
{
    public string GetActionName() => "PronunciationFeedback";
    
    public bool CanExecute(LLMActionContext context)
    {
        return !string.IsNullOrEmpty(context.UserInput);
    }
    
    public async Task<LLMActionResult> ExecuteAsync(ILLMService llm, LLMActionContext context)
    {
        string prompt = $"Provide pronunciation feedback for: {context.UserInput}";
        string response = await llm.GenerateResponseAsync(prompt, context.ConversationHistory);
        return LLMActionResult.CreateSuccess(response);
    }
}
```

---

## Configuration System

All hardcoded values have been replaced with **ScriptableObjects** for easy configuration without code changes.

### Creating Configuration Assets

1. Right-click in Project window
2. **Create → Language Tutor → [Config Type]**

### LLMConfig
- Service URL and endpoint
- Model name (llama3, gpt-4, etc.)
- Temperature, max tokens
- Retry settings
- **System Prompts** for each mode (Chat, Grammar, Vocabulary, Conversation)

### TTSConfig
- Service URL and endpoint
- Default voice and language
- Speech rate and pitch
- Audio caching settings
- Sample rate

### STTConfig
- Provider selection
- Max recording duration
- Sample rate
- Voice Activity Detection (VAD)
- Pronunciation assessment settings

### ConversationConfig
- Max history length
- Target language and user level (A1-C2)
- Enable grammar correction
- Enable vocabulary suggestions
- Auto-play TTS
- Show subtitles

---

## Usage Guide

### Basic Setup

1. **Create Configuration Assets**
   ```
   Assets → Create → Language Tutor → LLM Config
   Assets → Create → Language Tutor → TTS Config
   Assets → Create → Language Tutor → STT Config
   Assets → Create → Language Tutor → Conversation Config
   ```

2. **Add NPCController to Scene**
   - Drag `NPCController.cs` onto a GameObject
   - Assign all 4 configuration assets
   - Assign `WhisperManager` reference

3. **Add NPCView to Scene**
   - Drag `NPCView.cs` onto a UI GameObject
   - Assign TextMeshProUGUI for subtitles
   - Assign Button for talk button
   - Assign AudioSource for playback
   - Drag this NPCView to NPCController's `npcView` field

4. **Configure Services**
   - Ensure Ollama is running with llama3 model: `ollama run llama3`
   - Ensure AllTalk TTS server is running on port 7851
   - WhisperManager should be configured in scene

### Switching Action Modes

```csharp
// From another script:
npcController.SetActionMode(ActionMode.GrammarCheck);

// Or set in Inspector:
Default Action Mode → Grammar Check
```

### Adding Custom Actions

1. Create new class implementing `ILLMAction`
2. Add new enum value to `ActionMode` in NPCController
3. Add case in `SetActionMode()` method

```csharp
case ActionMode.PronunciationFeedback:
    _currentAction = new PronunciationFeedbackAction();
    break;
```

---

## Event System

Components communicate through events for loose coupling:

### AudioInputController Events
- `OnRecordingStarted` - Recording begins
- `OnRecordingCompleted(AudioClip)` - Audio ready for processing
- `OnRecordingError(string)` - Recording failed

### ConversationPipeline Events
- `OnStageChanged(PipelineStage)` - Pipeline stage updated
- `OnTranscriptionCompleted(string)` - Speech converted to text
- `OnLLMResponseReceived(string)` - AI response generated
- `OnTTSAudioGenerated(AudioClip)` - Audio synthesized
- `OnPipelineError(string)` - Error occurred

---

## Error Handling

### Automatic Retries
- LLM requests: Configurable retries (default: 2)
- Exponential backoff between retries
- Detailed error logging

### Fallback Strategies
- Empty transcription → Show "I didn't hear anything"
- LLM timeout → Retry with backoff
- TTS failure → Show error, allow retry

### Service Health Checks
```csharp
bool isAvailable = await llmService.IsAvailableAsync();
```

---

## Testing Strategy

### Unit Testing (Recommended)
```csharp
// Mock services for testing
public class MockLLMService : ILLMService
{
    public Task<string> GenerateResponseAsync(string prompt, ...)
    {
        return Task.FromResult("Mock response");
    }
}

// Test actions in isolation
var action = new ChatAction();
var mockService = new MockLLMService();
var result = await action.ExecuteAsync(mockService, context);
```

### Integration Testing
- Test full pipeline with real services
- Validate conversation history retention
- Test action switching

---

## Migration from Old Code

### Old NPCManager (193 lines, 7 responsibilities)
```
Removed: NPCManager.cs
```

### New Architecture (Clean separation)
```
NPCController.cs       - 280 lines (orchestration)
ConversationPipeline.cs - 180 lines (pipeline logic)
AudioInputController.cs - 160 lines (recording)
NPCView.cs             - 200 lines (UI/audio)
```

**Benefits**:
- Each component is independently testable
- Easy to swap implementations (e.g., switch from Ollama to OpenAI)
- Clear boundaries between concerns
- Conversation history for context-aware responses

---

## Performance Optimizations

### Audio Caching
AllTalkService caches generated audio clips to avoid re-generation of repeated phrases.

### History Trimming
ConversationHistory automatically summarizes old messages when exceeding max length.

### Async/Await
All I/O operations use async patterns to prevent UI blocking.

---

## Future Enhancements

### Planned Features
1. **Multi-NPC Support**: NPCRegistry to manage multiple characters
2. **Conversation Scenarios**: Guided dialogue trees (ScriptableObjects)
3. **Progress Tracking**: User stats, session history, achievements
4. **Pronunciation Scoring**: Detailed phoneme-level feedback
5. **AR Integration**: Spatial audio, world-locked NPCs, hand tracking
6. **Cloud LLM Support**: OpenAI, Azure Cognitive Services fallbacks
7. **Vocabulary Database**: Spaced repetition system

### Extension Points
- Implement `ILLMService` for new providers (OpenAI, Azure, etc.)
- Create custom `ILLMAction` for specific learning scenarios
- Add new `ScriptableObject` configs for additional features

---

## Troubleshooting

### "LLM request failed: Model not found"
```bash
# Pull the model
ollama pull llama3
```

### "TTS request failed: Connection refused"
```bash
# Start AllTalk TTS server
cd path/to/alltalk
python server.py
```

### "No microphone devices found"
- Check Windows Privacy Settings → Microphone access
- Verify Unity has microphone permissions

### "Configuration assets are missing"
- Create all 4 config ScriptableObjects
- Assign them in NPCController Inspector

---

## Code Quality Improvements

### Before Refactoring
- ❌ God Object (NPCManager doing everything)
- ❌ Hardcoded URLs and prompts
- ❌ No conversation context
- ❌ Tight coupling to specific services
- ❌ Mixed async paradigms
- ❌ No action extensibility

### After Refactoring
- ✅ Single Responsibility Principle
- ✅ ScriptableObject configuration
- ✅ Multi-turn conversation history
- ✅ Interface-based services (swappable)
- ✅ Consistent async/await
- ✅ Generic action system (Command Pattern)

---

## Dependencies

- **Unity 6000.0.3f1+**
- **Whisper.Unity** (STT) - GitHub package
- **TextMeshPro** (UI)
- **Meta XR SDK** (AR/VR)
- **Ollama** (LLM) - Local server
- **AllTalk TTS** (TTS) - Local server

---

## Contributing

When adding new features:
1. Follow existing patterns (services, actions, configs)
2. Use interfaces for extensibility
3. Add configuration to ScriptableObjects
4. Emit events for loose coupling
5. Add XML documentation comments

---

## License

[Your License Here]

---

## Contact

For questions or issues, please refer to the project README or create an issue in the repository.
