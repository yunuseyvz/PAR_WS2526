# Creating Your First Interactive NPC - Step by Step

## Current Scene Status

I can see you have:
- ✅ Scene: `NPC_Test`
- ✅ Character: `Character_0413da` (with animations)
- ✅ Old `NPCManager` script attached (we'll replace this)

## Step-by-Step Setup

### Step 1: Create Configuration Assets

1. **In the Project window**, navigate to `Assets/_Project/Scripts/Data`
2. **Right-click** in the Project window → **Create** → **Language Tutor** → **LLM Config**
   - Name it: `DefaultLLMConfig`
3. **Right-click** → **Create** → **Language Tutor** → **TTS Config**
   - Name it: `DefaultTTSConfig`
4. **Right-click** → **Create** → **Language Tutor** → **STT Config**
   - Name it: `DefaultSTTConfig`
5. **Right-click** → **Create** → **Language Tutor** → **Conversation Config**
   - Name it: `DefaultConversationConfig`

### Step 2: Configure the Assets

#### DefaultLLMConfig
1. **Select** `DefaultLLMConfig` in Project window
2. **In Inspector**, set:
   ```
   Service URL: http://127.0.0.1:11434
   Endpoint Path: /api/generate
   Model Name: llama3
   Max Tokens: 512
   Temperature: 0.7
   Timeout Seconds: 30
   Max Retries: 2
   ```
3. **System Prompts** (leave default or customize):
   - Default System Prompt: "You are a helpful language learning assistant..."
   - Conversation Practice Prompt: "You are a native speaker engaging in conversation..."

#### DefaultTTSConfig
1. **Select** `DefaultTTSConfig`
2. **In Inspector**, set:
   ```
   Service URL: http://127.0.0.1:7851
   Endpoint Path: /api/tts-generate
   Default Voice: male_01.wav
   Default Language: en
   Speech Rate: 1.0
   Sample Rate: 22050
   Enable Caching: ✓ (checked)
   Max Cache Size: 50
   ```

#### DefaultSTTConfig
1. **Select** `DefaultSTTConfig`
2. **In Inspector**, set:
   ```
   Provider: Whisper
   Default Language: en
   Max Recording Duration: 30
   Sample Rate: 44100
   Silence Threshold: 0.01
   ```

#### DefaultConversationConfig
1. **Select** `DefaultConversationConfig`
2. **In Inspector**, set:
   ```
   Max History Length: 20
   Target Language: en
   User Level: A2_Elementary
   Enable Grammar Correction: ✓
   Show Subtitles: ✓
   Auto Play TTS: ✓
   Response Delay: 0.5
   ```

### Step 3: Setup the NPC GameObject

#### Option A: Using Your Existing Character

1. **In Hierarchy**, select `Character_0413da`
2. **In Inspector**, find the OLD `NPC Manager (Script)` component
3. **Remove it**: Click the ⚙️ icon → **Remove Component**
4. **Add new component**: Click **Add Component** → Type `NPCController` → Select `LanguageTutor.Core.NPCController`

#### Option B: Clean Setup (Recommended)

1. **In Hierarchy**, create empty GameObject: Right-click → **Create Empty**
2. **Name it**: `NPCSystem`
3. **Add Component**: `LanguageTutor.Core.NPCController`

### Step 4: Configure NPCController

**With `NPCSystem` (or `Character_0413da`) selected**, in Inspector:

1. **Configuration Section:**
   - Drag `DefaultLLMConfig` → **Llm Config** field
   - Drag `DefaultTTSConfig` → **Tts Config** field
   - Drag `DefaultSTTConfig` → **Stt Config** field
   - Drag `DefaultConversationConfig` → **Conversation Config** field

2. **Components Section:**
   - Drag `WhisperManager` from Hierarchy → **Whisper Manager** field
   
3. **Action Mode:**
   - Set **Default Action Mode** to: `Chat`

### Step 5: Create the UI (NPCView)

1. **In Hierarchy**, find or create a **Canvas**:
   - If none exists: Right-click Hierarchy → **UI** → **Canvas**
   
2. **Under Canvas**, create empty GameObject:
   - Right-click Canvas → **Create Empty**
   - Name it: `NPCView`
   - **Add Component**: `LanguageTutor.UI.NPCView`

3. **Create Subtitle Text**:
   - Right-click `NPCView` → **UI** → **Text - TextMeshPro**
   - Name it: `SubtitleText`
   - **Position**: Set anchors to **bottom-center** (hold Shift+Alt, click bottom-center preset)
   - **Rect Transform**:
     ```
     Pos Y: 100
     Width: 800
     Height: 100
     ```
   - **TextMeshPro Settings**:
     ```
     Font Size: 24
     Alignment: Center/Middle
     Color: White
     Enable Word Wrapping: ✓
     ```
   - **Drag** this to NPCView's **Subtitle Text** field

4. **Create Talk Button**:
   - Right-click `NPCView` → **UI** → **Button - TextMeshPro**
   - Name it: `TalkButton`
   - **Position**: Set anchors to **bottom-center**
   - **Rect Transform**:
     ```
     Pos Y: 50
     Width: 200
     Height: 60
     ```
   - **Change button text** (select child Text object):
     - Text: `Talk`
     - Font Size: 24
   - **Drag** the button to NPCView's **Talk Button** field

5. **Create Status Indicator** (Optional but recommended):
   - Right-click `NPCView` → **UI** → **Image**
   - Name it: `StatusIndicator`
   - **Position**: Top-right corner
   - **Rect Transform**:
     ```
     Pos X: -50
     Pos Y: -50
     Width: 30
     Height: 30
     ```
   - **Image Settings**:
     - Source Image: (use any circle sprite, or leave white square)
     - Color: White
   - **Drag** to NPCView's **Status Indicator** field

6. **Add Audio Source to NPCView**:
   - **Select** `NPCView` GameObject
   - **Add Component**: **Audio Source**
   - **Settings**:
     ```
     Play On Awake: ✗ (unchecked)
     Loop: ✗ (unchecked)
     Spatial Blend: 0 (2D sound)
     ```
   - This should **auto-assign** to NPCView's **Audio Source** field
   - If not, drag the NPCView GameObject to itself → it will find the Audio Source

### Step 6: Link NPCView to NPCController

1. **Select** your `NPCSystem` (or `Character_0413da`) GameObject
2. **In Inspector**, find `NPCController` component
3. **Drag** the `NPCView` GameObject → **Npc View** field

### Step 7: Verify Setup

**Your NPCController Inspector should show:**
```
✓ Llm Config: DefaultLLMConfig
✓ Tts Config: DefaultTTSConfig
✓ Stt Config: DefaultSTTConfig
✓ Conversation Config: DefaultConversationConfig
✓ Whisper Manager: WhisperManager
✓ Npc View: NPCView
✓ Default Action Mode: Chat
```

**Your NPCView Inspector should show:**
```
✓ Subtitle Text: SubtitleText (TextMeshProUGUI)
✓ Talk Button: TalkButton (Button)
✓ Status Indicator: StatusIndicator (Image)
✓ Audio Source: Audio Source (on NPCView)
```

### Step 8: Start External Services

Before testing, ensure both services are running:

#### Start Ollama (Terminal/Command Prompt):
```bash
# If not running, start it
ollama serve

# Verify model is available
ollama list
# Should show llama3

# If llama3 not listed:
ollama pull llama3
```

#### Start AllTalk TTS (Terminal/Command Prompt):
```bash
cd path/to/alltalk_tts
python server.py
# Server should start on http://127.0.0.1:7851
```

### Step 9: Test Your NPC!

1. **Press Play** ▶️ in Unity Editor
2. **Check Console** for initialization logs:
   ```
   [NPCController] Services initialized - LLM: llama3, TTS: male_01.wav
   [NPCController] Core systems initialized
   [NPCController] Initialized successfully
   ```

3. **Click "Talk" button** - Status indicator should turn **yellow** (listening)
4. **Speak into your microphone**: "Hello, how are you?"
5. **Click "Talk" again** to stop recording
6. **Watch the pipeline**:
   - Status turns **blue** - "Transcribing..."
   - Subtitle shows: "Player: Hello, how are you?"
   - Status stays **blue** - "Thinking..."
   - Subtitle shows: "NPC: [AI response]"
   - Status turns **blue** - "Generating voice..."
   - Status turns **green** - "NPC is speaking..."
   - Audio plays!

### Troubleshooting

#### "Configuration assets are missing"
- Make sure all 4 ScriptableObjects are created and assigned in NPCController

#### "WhisperManager is not assigned"
- Find WhisperManager in your scene hierarchy and drag it to the field

#### Console shows "LLM request failed: Connection refused"
- Start Ollama: `ollama serve`

#### Console shows "TTS request failed: Connection refused"
- Start AllTalk TTS server: `python server.py`

#### "AllTalk request failed: HTTP/1.1 500 Internal Server Error" or "Model file not found"
This means AllTalk TTS is running but doesn't have voice models configured.

**Fix AllTalk TTS Model Setup:**

1. **Stop AllTalk server** (Ctrl+C in terminal)

2. **Navigate to AllTalk folder:**
   ```bash
   cd path/to/alltalk_tts
   ```

3. **Check if models exist:**
   ```bash
   # Look for models in these folders:
   dir models\      # Windows
   ls models/       # Linux/Mac
   ```

4. **If models folder is empty or missing models:**
   - Open AllTalk web interface: `http://127.0.0.1:7851`
   - Go to **Settings** or **Model Management**
   - Click **Download Models** or **Install TTS Models**
   - Wait for download to complete (may take several minutes)

5. **Alternative - Manual model download:**
   ```bash
   # In AllTalk directory
   python download_models.py
   # OR
   python setup.py
   ```

6. **Configure the voice in confignew.json or config.json:**
   - Open `confignew.json` in AllTalk folder
   - Find `"model_name"` setting
   - Make sure it matches an existing model file
   - Common models: `"xtts"`, `"vits"`, etc.

7. **Restart AllTalk server:**
   ```bash
   python server.py
   ```

8. **Verify it works:**
   - Open browser: `http://127.0.0.1:7851`
   - Try generating test audio in the web interface
   - If web interface works, Unity should work too

9. **Update Unity TTS Config if needed:**
   - In Unity, select `DefaultTTSConfig`
   - Change **Default Voice** to match available voice files
   - Common AllTalk voices: `"female_01.wav"`, `"male_01.wav"`, or just `"default"`

10. **Still not working? Check AllTalk console output:**
    - Look for available voices listed when server starts
    - Use one of those voice names in Unity's TTSConfig

#### "I didn't hear anything" / Empty transcription
- Check microphone is working in Windows Settings
- Speak louder or closer to mic
- Check Console for detailed error messages

#### No audio plays after NPC responds
- Check NPCView has Audio Source component
- Check "Auto Play TTS" is enabled in ConversationConfig
- Check your speakers/headphones are connected

**Audio Troubleshooting Checklist:**

1. **Verify Audio Listener exists in scene:**
   - Every Unity scene NEEDS an Audio Listener (usually on Main Camera)
   - In Hierarchy, check if your Camera has "Audio Listener" component
   - If missing: Select Main Camera → Add Component → Audio Listener

2. **Check Audio Source settings on NPCView:**
   - Select NPCView GameObject in Hierarchy
   - Audio Source settings should be:
     ```
     Volume: 1.0 (slider all the way right)
     Mute: ✗ (unchecked)
     Play On Awake: ✗ (unchecked)
     Loop: ✗ (unchecked)
     Spatial Blend: 0.0 (slider all the way left = 2D)
     ```

3. **Check Unity Editor Audio Settings:**
   - Top menu: Edit → Project Settings → Audio
   - Make sure "Disable Unity Audio" is ✗ (unchecked)
   - Check "System Sample Rate" matches your system
   - In Game view, make sure mute button (🔊) is not active

4. **Test with simple audio:**
   - Add any audio clip to NPCView's Audio Source "AudioClip" field
   - Check "Play On Awake"
   - Press Play - should hear audio immediately
   - If you don't hear this, Unity audio is broken

5. **Check Windows Audio Mixer:**
   - Open Windows Sound Settings
   - Check Unity Editor is not muted in Volume Mixer
   - Check correct output device is selected

6. **Verify audio file is valid:**
   - Look at Console for: `[NPCView] Playing audio clip: [name], length: [X]s`
   - If length is 0.0s, the WAV file is corrupted
   - If length > 0s but no sound, it's an output issue

7. **Force audio device refresh:**
   ```csharp
   // Add this debug line in NPCView.PlayAudio() temporarily:
   Debug.Log($"Audio settings - Sample rate: {AudioSettings.outputSampleRate}, speakers: {AudioSettings.speakerMode}");
   ```

8. **Last resort - Restart Unity:**
   - Save scene
   - Close Unity completely
   - Reopen project
   - Sometimes Unity's audio gets stuck

### Next Steps

#### Change NPC Personality
Edit system prompts in `DefaultLLMConfig`:
- Make it funny, serious, teacher-like, etc.

#### Switch to Grammar Correction Mode
In NPCController Inspector:
- Change **Default Action Mode** to: `Grammar Check`
- Now the NPC will correct your grammar!

#### Test Other Modes
- `Vocabulary Teach` - Ask about words: "What does 'serendipity' mean?"
- `Conversation Practice` - Natural dialogue practice

#### Add Visual Feedback to Your Character
If you want the character model to react:
1. Keep `Character_0413da` in scene
2. Add a script that listens to NPCController events
3. Trigger animations when NPC speaks

```csharp
// Example animation trigger script
public class NPCAnimator : MonoBehaviour
{
    public Animator animator;
    public NPCController npcController;
    
    void Start()
    {
        // Subscribe to pipeline events
        var pipeline = npcController.GetComponent<ConversationPipeline>();
        // Trigger animations based on pipeline stage
    }
}
```

## Summary

You now have a fully functional AI language tutor NPC that:
- ✅ Listens to your voice
- ✅ Transcribes speech to text (Whisper)
- ✅ Generates intelligent responses (Llama3)
- ✅ Speaks responses with realistic voice (AllTalk TTS)
- ✅ Remembers conversation history
- ✅ Can switch between different teaching modes

Have fun talking to your NPC! 🎮🤖💬
