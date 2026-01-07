# Quick Setup Guide - AR Language Tutor

## Step-by-Step Setup

### 1. Create Configuration Assets

Create the following ScriptableObject assets (right-click in Project window):

```
Create → Language Tutor → LLM Config
Create → Language Tutor → TTS Config  
Create → Language Tutor → STT Config
Create → Language Tutor → Conversation Config
```

**Recommended Names:**
- `DefaultLLMConfig`
- `DefaultTTSConfig`
- `DefaultSTTConfig`
- `DefaultConversationConfig`

### 2. Configure LLM Settings

**DefaultLLMConfig:**
- Service URL: `http://127.0.0.1:11434`
- Endpoint Path: `/api/generate`
- Model Name: `llama3`
- Max Tokens: `512`
- Temperature: `0.7`
- Max Retries: `2`

**System Prompts** (customize these for your needs):
- Default: "You are a helpful language learning assistant..."
- Grammar Correction: "You are a language tutor focused on grammar..."
- Vocabulary Teaching: "You are a vocabulary tutor..."
- Conversation Practice: "You are a native speaker engaging in conversation..."

### 3. Configure TTS Settings

**DefaultTTSConfig:**
- Service URL: `http://127.0.0.1:7851`
- Endpoint Path: `/api/tts-generate`
- Default Voice: `male_01.wav` (or your preferred voice)
- Default Language: `en`
- Speech Rate: `1.0`
- Enable Caching: `✓` (checked)
- Max Cache Size: `50`

### 4. Configure STT Settings

**DefaultSTTConfig:**
- Provider: `Whisper`
- Default Language: `en`
- Max Recording Duration: `30` seconds
- Sample Rate: `44100` Hz
- Silence Threshold: `0.01`

### 5. Configure Conversation Settings

**DefaultConversationConfig:**
- Max History Length: `20` messages
- Target Language: `en`
- User Level: `A2_Elementary` (adjust based on your needs)
- Enable Grammar Correction: `✓`
- Enable Vocabulary Suggestions: `✓`
- Show Subtitles: `✓`
- Auto Play TTS: `✓`

### 6. Setup Scene

#### Add NPCController

1. Create empty GameObject: `NPCController`
2. Add Component → `LanguageTutor.Core.NPCController`
3. In Inspector:
   - Drag `DefaultLLMConfig` to **LLM Config**
   - Drag `DefaultTTSConfig` to **TTS Config**
   - Drag `DefaultSTTConfig` to **STT Config**
   - Drag `DefaultConversationConfig` to **Conversation Config**
   - Find and drag `WhisperManager` to **Whisper Manager**
   - Set **Default Action Mode** to `Chat`

#### Add NPCView (UI)

1. Create UI Canvas if not exists: `GameObject → UI → Canvas`
2. Create empty GameObject under Canvas: `NPCView`
3. Add Component → `LanguageTutor.UI.NPCView`

4. **Create UI Elements:**
   
   **Subtitle Text:**
   - Create `GameObject → UI → TextMeshPro - Text`
   - Name: `SubtitleText`
   - Set anchor to bottom-center
   - Drag to NPCView's **Subtitle Text** field

   **Talk Button:**
   - Create `GameObject → UI → Button - TextMeshPro`
   - Name: `TalkButton`
   - Position at bottom center
   - Set button text to "Talk"
   - Drag to NPCView's **Talk Button** field

   **Status Indicator (Optional):**
   - Create `GameObject → UI → Image`
   - Name: `StatusIndicator`
   - Small circle/square in corner
   - Drag to NPCView's **Status Indicator** field

   **Audio Source:**
   - Add Audio Source component to NPCView GameObject
   - Drag to NPCView's **Audio Source** field

5. **Link NPCView to NPCController:**
   - Drag NPCView GameObject to NPCController's **Npc View** field

### 7. Start External Services

#### Start Ollama (LLM)
```bash
# Install Ollama from https://ollama.ai
# Pull the llama3 model
ollama pull llama3

# Run Ollama server (usually starts automatically)
ollama serve
```

#### Start AllTalk TTS
```bash
# Navigate to AllTalk directory
cd path/to/alltalk_tts

# Start the server
python server.py
# Server should be running on http://127.0.0.1:7851
```

### 8. Test the Setup

1. **Press Play** in Unity Editor
2. **Click "Talk" button** - Should show "Listening..."
3. **Speak into microphone**
4. **Click "Talk" again** to stop recording
5. Watch the pipeline:
   - "Transcribing..." - Converting speech to text
   - "Thinking..." - Getting LLM response  
   - "Generating voice..." - Creating TTS audio
   - "NPC is speaking..." - Playing audio

### 9. Verify Each Component

**Check Console Logs:**
```
[NPCController] Services initialized - LLM: llama3, TTS: male_01.wav
[NPCController] Core systems initialized
[NPCController] Initialized successfully
[AudioInputController] Initialized microphone: [Your Mic Name]
```

**Test Each Stage:**
1. **Recording**: Status indicator should turn yellow
2. **Transcription**: Should see "Player: [your words]" in subtitles
3. **LLM Response**: Should see "NPC: [AI response]" in subtitles
4. **Audio Playback**: Should hear synthesized speech

---

## Common Issues & Solutions

### Issue: "Configuration assets are missing"
**Solution:** Create all 4 ScriptableObject configs and assign them in Inspector

### Issue: "WhisperManager is not assigned"
**Solution:** Find WhisperManager in scene (usually on a GameObject) and drag to field

### Issue: "LLM request failed: Connection refused"
**Solution:** Start Ollama server: `ollama serve`

### Issue: "TTS request failed: Connection refused"  
**Solution:** Start AllTalk server: `python server.py` in AllTalk directory

### Issue: "No microphone devices found"
**Solution:** Check Windows Settings → Privacy → Microphone → Allow apps to access

### Issue: Empty transcription / "I didn't hear anything"
**Solution:** 
- Check microphone is working
- Speak louder or closer to mic
- Increase recording duration in STTConfig

### Issue: "Model not found" error
**Solution:** Pull the model: `ollama pull llama3`

---

## Switching Action Modes

### Via Inspector (Before Runtime)
Set **Default Action Mode** in NPCController:
- `Chat` - General conversation
- `Grammar Check` - Grammar correction feedback
- `Vocabulary Teach` - Word definitions and examples
- `Conversation Practice` - Scenario-based dialogue

### Via Code (During Runtime)
```csharp
// Get reference to controller
var controller = FindObjectOfType<NPCController>();

// Switch mode
controller.SetActionMode(ActionMode.GrammarCheck);
```

### Creating UI Buttons for Mode Switching
```csharp
// Add buttons in your UI
public Button chatButton;
public Button grammarButton;
public Button vocabButton;

void Start()
{
    chatButton.onClick.AddListener(() => 
        controller.SetActionMode(ActionMode.Chat));
    
    grammarButton.onClick.AddListener(() => 
        controller.SetActionMode(ActionMode.GrammarCheck));
    
    vocabButton.onClick.AddListener(() => 
        controller.SetActionMode(ActionMode.VocabularyTeach));
}
```

---

## Next Steps

1. **Customize System Prompts** in LLMConfig for your specific language learning goals
2. **Adjust User Level** in ConversationConfig (A1-C2) to match your target learners
3. **Change Target Language** in ConversationConfig for non-English learning
4. **Experiment with Action Modes** to see different teaching styles
5. **Monitor Conversation History** using `controller.GetConversationSummary()`
6. **Create Custom Actions** for specialized learning scenarios (see ARCHITECTURE.md)

---

## Performance Tips

1. **Enable Audio Caching** in TTSConfig for repeated phrases
2. **Adjust History Length** in ConversationConfig (lower = faster, less context)
3. **Reduce Max Tokens** in LLMConfig for shorter responses (faster generation)
4. **Use Lower Sample Rate** in STTConfig if transcription is slow (e.g., 22050 Hz)

---

## Development Workflow

### Typical Iteration Cycle:
1. Modify system prompts in LLMConfig
2. Test in Play mode
3. Adjust temperature/max tokens for response style
4. Switch action modes to test different behaviors
5. Review conversation history for context accuracy

### Debugging:
- Check Console for detailed logs from each component
- Use `Debug` mode in configs for verbose logging
- Monitor subtitle text for pipeline progress
- Test services individually using health checks

---

## Ready to Extend?

See [ARCHITECTURE.md](ARCHITECTURE.md) for:
- Creating custom LLM actions
- Adding new service providers
- Implementing conversation scenarios
- Multi-NPC support
- Progress tracking systems

Happy language tutoring! 🎓✨
