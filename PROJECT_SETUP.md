# AR Language Tutor - Complete Setup Guide

An **AR/VR language learning application** powered by AI for immersive conversational practice with virtual NPCs on Meta Quest 3.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3.0f1-black.svg)](https://unity.com/)
[![Meta XR SDK](https://img.shields.io/badge/Meta%20XR%20SDK-v83.0.1-blue.svg)](https://developer.oculus.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Installation](#installation)
4. [External Services Setup](#external-services-setup)
5. [Unity Project Configuration](#unity-project-configuration)
6. [Scene Setup](#scene-setup)
7. [VR/AR Setup (Meta Quest 3)](#vrar-setup-meta-quest-3)
8. [Testing & Troubleshooting](#testing--troubleshooting)
9. [Project Architecture](#project-architecture)
10. [Usage](#usage)

---

## 🎯 Overview

This Unity project combines:
- **Augmented Reality (Meta Quest 3)** for immersive learning environments
- **Ollama + Llama3** for intelligent conversational AI (LLM)
- **Whisper.Unity** for accurate speech-to-text recognition
- **AllTalk TTS** for natural text-to-speech synthesis

The application provides an interactive language tutor NPC that can:
- ✅ Engage in natural conversations
- ✅ Correct grammar mistakes
- ✅ Teach vocabulary with context
- ✅ Provide pronunciation feedback
- ✅ Adapt to different learning scenarios

---

## 📦 Prerequisites

### Required Software

1. **Unity Hub & Editor**
   - Version: `Unity 6000.0.3f1` (Unity 6)
   - [Download Unity Hub](https://unity.com/download)

2. **Git & Git LFS**
   - [Download Git](https://git-scm.com/downloads)
   - [Download Git LFS](https://git-lfs.com/)
   - Required for large asset files (models, audio)

3. **Ollama (LLama3 default, can be freely changed)**
   - [Download Ollama](https://ollama.ai/)
   - After installation, run: `ollama pull llama3`
   - Runs local LLM on port `11434`

4. **AllTalk TTS**
   - [AllTalk TTS (Beta Branch)](https://github.com/erew123/alltalk_tts/tree/alltalkbeta)
   - Follow their installation instructions
   - Runs on port `7851`

### Hardware Requirements

- **For Development**: Windows PC with 16GB+ RAM, GPU recommended
- **For Deployment**: Meta Quest 3 headset
- **Microphone**: Built-in Quest 3 mic or external

### Unity Packages (Auto-installed)

These packages are included in the project manifest:
- Meta XR SDK v83.0.1
- TextMeshPro
- Whisper.Unity (from GitHub)
- Input System

---

## 🚀 Installation

### Step 1: Install Git LFS

```bash
git lfs install
```

### Step 2: Clone the Repository

```bash
git clone https://github.com/yourusername/PAR_WS2526.git
cd PAR_WS2526
```

### Step 3: Pull LFS Assets

```bash
git lfs pull
```

### Step 4: Open in Unity

1. Open **Unity Hub**
2. Click **"Add"** → Select the cloned `PAR_WS2526` folder
3. Unity version should match `6000.0.3f1` (Unity 6)
4. Click the project to open it
5. Wait for package imports and compilation (may take 5-10 minutes first time)

---

## ⚙️ External Services Setup

### 1. Start Ollama with Llama3

```bash
# Pull the model (first time only)
ollama pull llama3

# Start Ollama server
ollama serve
```

Verify it's running at: `http://localhost:11434`

### 2. Start AllTalk TTS

Follow [AllTalk TTS installation guide](https://github.com/erew123/alltalk_tts/tree/alltalkbeta):

```bash
# Navigate to AllTalk directory
cd alltalk_tts

# Activate environment (if using conda/venv)
conda activate alltalk  # or source venv/bin/activate

# Start the server
python server.py
```

Verify it's running at: `http://localhost:7851`

### 3. Verify Services

Test both services are accessible:

**Ollama:**
```bash
curl http://localhost:11434/api/generate -d '{"model":"llama3","prompt":"Hello"}'
```

**AllTalk:**
- Open browser to `http://localhost:7851`
- You should see the AllTalk web interface

---

## 🎮 Unity Project Configuration

### Step 1: Create Configuration Assets

1. **In Unity Project window**, navigate to: `Assets/_Project/Scripts/Data/`

2. **Create ScriptableObjects** (Right-click in Project window):
   ```
   Create → Language Tutor → LLM Config → Name: "DefaultLLMConfig"
   Create → Language Tutor → TTS Config → Name: "DefaultTTSConfig"
   Create → Language Tutor → STT Config → Name: "DefaultSTTConfig"
   Create → Language Tutor → Conversation Config → Name: "DefaultConversationConfig"
   ```

### Step 2: Configure LLM Settings

**Select `DefaultLLMConfig`** and set:

```
LLM Service Configuration:
├─ Service URL: http://127.0.0.1:11434
├─ Endpoint Path: /api/generate
├─ Model Name: llama3
├─ Max Tokens: 512
├─ Temperature: 0.7
├─ Stream Response: ✓ (checked)
└─ Max Retries: 2

System Prompts:
├─ Default Prompt: "You are a helpful language learning assistant..."
├─ Grammar Correction: "You are a language tutor focused on grammar..."
├─ Vocabulary Teaching: "You are a vocabulary tutor teaching new words..."
└─ Conversation Practice: "You are a native speaker engaging in natural conversation..."
```

**Tip**: Customize prompts based on target language and learning goals.

### Step 3: Configure TTS Settings

**Select `DefaultTTSConfig`** and set:

```
TTS Service Configuration:
├─ Service URL: http://127.0.0.1:7851
├─ Endpoint Path: /api/tts-generate
├─ Default Voice: male_01.wav  (or your preferred voice)
├─ Default Language: en
├─ Speech Rate: 1.0
├─ Enable Caching: ✓
└─ Max Cache Size: 50
```

**Available voices**: Check AllTalk's `voices/` folder for options (male_01, female_01, etc.)

### Step 4: Configure STT Settings

**Select `DefaultSTTConfig`** and set:

```
STT Service Configuration:
├─ Recording Settings:
│  ├─ Max Recording Duration: 10 seconds
│  ├─ Sample Rate: 16000
│  └─ Auto Stop On Silence: ✓
│
└─ Whisper Settings:
   ├─ Model: base.en (or larger for better accuracy)
   └─ Language: en
```

### Step 5: Configure Conversation Settings

**Select `DefaultConversationConfig`** and set:

```
Conversation Configuration:
├─ Max History Length: 10 (last 10 exchanges)
├─ Default Action Mode: Chat
├─ Enable Context: ✓
├─ Show Subtitles: ✓
└─ Auto Play Response: ✓
```

---

## 🏗️ Scene Setup

### Open the Scene

1. Navigate to: `Assets/Scenes/`
2. Open `NPC_Test.unity` (or your main scene)

### Scene Setup (From Scratch)

If starting fresh:

#### 1. Add Meta Quest VR Camera Rig

1. **In Hierarchy**: Right-click → **XR** → **Meta Quest** → **OVR Camera Rig**
2. Delete default Main Camera
3. Position OVRCameraRig at origin: `(0, 0, 0)`

#### 2. Add Whisper Manager

1. **In Hierarchy**: Right-click → **Create Empty** → Name: `WhisperManager`
2. **Add Component** → Search: `Whisper Manager` (from Whisper.Unity package)
3. Configure:
   - Model: `base.en` or `small.en`
   - Language: `English`

#### 3. Add NPC Character

1. **In Project window**, navigate to:
   ```
   Assets/Assets/PartyPeople/Pack_FREE_PartyCharacters/Prefabs/
   ```
2. **Drag any character prefab** (e.g., `Character_0413da`) into scene
3. **Set Transform**:
   ```
   Position: X: 0, Y: 0, Z: 3  (3 meters in front of player)
   Rotation: X: 0, Y: 180, Z: 0  (facing camera)
   Scale: X: 1, Y: 1, Z: 1
   ```

#### 4. Add NPCController to Character

1. **Select your character** in Hierarchy
2. **Add Component** → `NPCController` (or type `LanguageTutor.Core.NPCController`)
3. **Add Component** → `NPCView` (handles UI and audio)
4. **Add Component** → `Audio Source` (for TTS playback)
   - Volume: 1.0
   - Spatial Blend: 0 (2D for now)

---

## 🥽 VR/AR Setup (Meta Quest 3)

### Step 1: Create VR HUD Canvas

1. **In Hierarchy**: Right-click → **UI** → **Canvas**
2. Rename to: `VRHudCanvas`
3. **In Canvas Inspector**:
   ```
   Canvas:
   ├─ Render Mode: World Space (change from Screen Space)
   └─ Event Camera: [Will assign in next step]
   ```

### Step 2: Parent Canvas to Camera

4. **Expand OVRCameraRig** in Hierarchy:
   ```
   OVRCameraRig
   └─ TrackingSpace
      └─ CenterEyeAnchor  ← This is the camera
   ```

5. **Drag `VRHudCanvas`** onto **CenterEyeAnchor** (make it a child)

6. **Set VRHudCanvas Transform** (local coordinates):
   ```
   Position: X: 0, Y: -0.2, Z: 1.5  (1.5m in front, slightly below eye level)
   Rotation: X: 0, Y: 0, Z: 0
   Scale: X: 0.001, Y: 0.001, Z: 0.001  (scales down to readable size)
   
   Rect Transform:
   ├─ Width: 800
   └─ Height: 400
   ```

7. **Assign Event Camera**:
   - In Canvas component, drag **CenterEyeAnchor** to Event Camera field

### Step 3: Build HUD UI Elements

#### A) Background Panel (Optional)

8. **Right-click VRHudCanvas** → **UI** → **Image**
   - Rename: `HudBackground`
   - **Rect Transform**: Anchor preset → Hold ALT+SHIFT → Click "Stretch All"
   - Margins: 10 on all sides
   - **Image**: Color → RGBA(0, 0, 0, 100) - dark semi-transparent
   - **Uncheck** "Raycast Target"

#### B) Subtitle Text

9. **Right-click VRHudCanvas** → **UI** → **Text - TextMeshPro**
   - (Import TMP Essentials if prompted)
   - Rename: `SubtitleText`
   - **Rect Transform**:
     ```
     Anchor: Top Center
     Position: X: 0, Y: -60
     Width: 700, Height: 120
     ```
   - **TextMeshPro Settings**:
     ```
     Text: "Subtitle text appears here..."
     Font Size: 28
     Alignment: Center (horizontal & vertical)
     Color: White
     Word Wrapping: ✓
     Overflow: Ellipsis
     ```

#### C) Talk Button

10. **Right-click VRHudCanvas** → **UI** → **Button - TextMeshPro**
    - Rename: `TalkButton`
    - **Rect Transform**:
      ```
      Anchor: Bottom Center
      Position: X: 0, Y: 40
      Width: 200, Height: 70
      ```
    - **Button Image**: Color → Blue (0, 120, 215)
    - **Expand TalkButton** → Select child "Text (TMP)"
      - Text: "🎤 TALK"
      - Font Size: 32
      - Color: White
      - Bold: ✓

#### D) Status Indicator

11. **Right-click VRHudCanvas** → **UI** → **Image**
    - Rename: `StatusIndicator`
    - **Rect Transform**:
      ```
      Anchor: Top Left
      Position: X: 40, Y: -40
      Width: 60, Height: 60
      ```
    - **Image**:
      - Color: Green (ready state)
      - Sprite: UI/UISprite
      - Preserve Aspect: ✓

#### E) Mode Text

12. **Right-click VRHudCanvas** → **UI** → **Text - TextMeshPro**
    - Rename: `ModeText`
    - **Rect Transform**:
      ```
      Anchor: Top Right
      Position: X: -120, Y: -40
      Width: 200, Height: 50
      ```
    - **TextMeshPro**:
      - Text: "Chat Mode"
      - Font Size: 24
      - Alignment: Middle Right
      - Color: Cyan

### Step 4: Connect UI to NPCController

13. **Select your NPC character** in Hierarchy

14. **In NPCView component**, assign:
    ```
    UI Components:
    ├─ Subtitle Text → Drag SubtitleText
    ├─ Talk Button → Drag TalkButton
    └─ Status Indicator → Drag StatusIndicator
    
    Audio:
    └─ Audio Source → Should auto-fill
    ```

15. **In NPCController component**, assign:
    ```
    Configuration:
    ├─ Default LLM Config → Drag DefaultLLMConfig
    ├─ Default TTS Config → Drag DefaultTTSConfig
    ├─ Default STT Config → Drag DefaultSTTConfig
    └─ Default Conversation Config → Drag DefaultConversationConfig
    
    Components:
    ├─ Npc View → Drag NPCView (same GameObject)
    └─ Whisper Manager → Drag WhisperManager from Hierarchy
    ```

### Step 5: Enable Controller Interaction

#### Option A: Add OVR Input Module (Recommended)

16. **In Hierarchy**, select **EventSystem**
17. **In Inspector**:
    - Remove "Standalone Input Module" if present
    - **Add Component** → `OVRInputModule`

#### Option B: Add Direct Button Input (Alternative)

18. **Select your NPC character**
19. **Add Component** → **New Script** → Name: `VRButtonInput`
20. **Copy this code**:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class VRButtonInput : MonoBehaviour
{
    [SerializeField] private Button talkButton;

    void Update()
    {
        // A button on right controller triggers recording
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            talkButton?.onClick.Invoke();
        }
    }
}
```

21. **In Inspector**, drag **TalkButton** to the script's Talk Button field

### Step 6: Add Audio Listener (Critical!)

22. **Select CenterEyeAnchor** in Hierarchy
23. **Check if it has "Audio Listener" component**
    - If missing: **Add Component** → **Audio Listener**
    - This is required to hear audio from the NPC's AudioSource!

---

## 🧪 Testing & Troubleshooting

### Pre-Flight Checklist

Before pressing Play:

- ✅ Ollama running on port 11434
- ✅ AllTalk TTS running on port 7851
- ✅ All Config ScriptableObjects created and assigned
- ✅ WhisperManager in scene
- ✅ NPCController properly configured
- ✅ VRHudCanvas parented to CenterEyeAnchor
- ✅ Audio Listener on CenterEyeAnchor
- ✅ Microphone permission granted in Unity (Edit → Project Settings → Player → Microphone Usage Description)

### Testing in Unity Editor

1. **Press Play** in Unity Editor
2. **Click the "🎤 TALK" button** with mouse (simulates VR controller)
3. **Speak into your microphone**
4. **Click button again** to stop recording
5. **Observe**:
   - Status indicator turns red (recording)
   - Subtitle shows your transcribed speech
   - Subtitle shows NPC response
   - NPC speaks (audio plays)

### Testing on Quest 3

#### Build Settings

1. **File** → **Build Settings**
2. **Switch Platform** to **Android**
3. **Add Open Scenes** (your current scene)
4. **Player Settings**:
   ```
   Company Name: [Your Name]
   Product Name: AR Language Tutor
   
   XR Plug-in Management:
   ├─ Oculus: ✓
   └─ Initialize XR on Startup: ✓
   
   Android Settings:
   ├─ Minimum API Level: 29
   ├─ Target API Level: 32+
   ├─ Package Name: com.yourcompany.languagetutor
   └─ Install Location: Auto
   ```

5. **Connect Quest 3** via USB (Developer Mode enabled)
6. **Build and Run** → Select output folder

### Common Issues & Solutions

#### 🔴 No Audio Playback

**Problem**: NPC text appears but no sound  
**Solution**: 
- Check Audio Listener exists on CenterEyeAnchor
- Verify AudioSource Volume = 1.0
- Check system volume / Quest volume

#### 🔴 Button Doesn't Respond

**Problem**: Clicking/pointing at button does nothing  
**Solution**:
- Verify OVRInputModule on EventSystem
- Check Canvas Event Camera is assigned
- Try VRButtonInput script (A button method)

#### 🔴 No Subtitles Appear

**Problem**: Recording works but no text shown  
**Solution**:
- Check SubtitleText is assigned in NPCView
- Verify Ollama/AllTalk services are running
- Check Unity Console for error messages

#### 🔴 Whisper Error: Model Not Found

**Problem**: "Whisper model not loaded"  
**Solution**:
- Select WhisperManager → Download model from Inspector
- Or manually place model in `StreamingAssets/Whisper/`

#### 🔴 LLM Connection Failed

**Problem**: "Connection refused to localhost:11434"  
**Solution**:
```bash
# Restart Ollama
ollama serve

# Verify it's running
curl http://localhost:11434/api/generate -d '{"model":"llama3","prompt":"Hi"}'
```

#### 🔴 TTS Error: 500 Internal Server Error

**Problem**: AllTalk returns error  
**Solution**:
- Check AllTalk server logs
- Verify voice file exists in AllTalk's `voices/` folder
- Try default voice: `male_01.wav`

#### 🔴 VR HUD Not Visible

**Problem**: UI doesn't appear in headset  
**Solution**:
- Check Canvas Scale is 0.001 (not 1.0)
- Verify Canvas is child of CenterEyeAnchor
- Check Z position is positive (1.5, not -1.5)

---

## 🏛️ Project Architecture

### Folder Structure

```
Assets/
├─ _Project/
│  ├─ Scripts/
│  │  ├─ Core/              (Main logic)
│  │  │  ├─ NPCController.cs
│  │  │  ├─ ConversationPipeline.cs
│  │  │  ├─ AudioInputController.cs
│  │  │  └─ ConversationHistory.cs
│  │  │
│  │  ├─ Services/          (External API wrappers)
│  │  │  ├─ ILLMService.cs
│  │  │  ├─ OllamaService.cs
│  │  │  ├─ ITTSService.cs
│  │  │  ├─ AllTalkService.cs
│  │  │  ├─ ISTTService.cs
│  │  │  └─ WhisperService.cs
│  │  │
│  │  ├─ Actions/           (LLM action modes)
│  │  │  ├─ ILLMAction.cs
│  │  │  ├─ ChatAction.cs
│  │  │  ├─ GrammarCheckAction.cs
│  │  │  ├─ VocabularyTeachAction.cs
│  │  │  └─ ConversationPracticeAction.cs
│  │  │
│  │  ├─ Data/              (Configuration ScriptableObjects)
│  │  │  ├─ LLMConfig.cs
│  │  │  ├─ TTSConfig.cs
│  │  │  ├─ STTConfig.cs
│  │  │  └─ ConversationConfig.cs
│  │  │
│  │  ├─ UI/                (View components)
│  │  │  └─ NPCView.cs
│  │  │
│  │  └─ Utilities/
│  │     └─ WavUtility.cs
│  │
│  └─ Documentation/
│     ├─ ARCHITECTURE.md
│     ├─ SETUP_GUIDE.md
│     └─ ENHANCED_NPC_SETUP.md
│
├─ Scenes/
│  └─ NPC_Test.unity
│
├─ Assets/                  (3rd party assets)
│  └─ PartyPeople/         (Character prefabs)
│
└─ Settings/               (Project settings)
```

### Key Design Patterns

- **Service-Oriented Architecture**: LLM, TTS, STT as injectable services
- **Command Pattern**: LLM actions (Chat, Grammar, Vocab, Practice) as swappable commands
- **MVC Pattern**: NPCController (Controller), ConversationPipeline (Model), NPCView (View)
- **Dependency Injection**: Services injected via ScriptableObject configs
- **Observer Pattern**: Events for state changes (recording start/stop, response ready)

### Component Flow

```
User Input (Voice) 
    ↓
AudioInputController (Microphone)
    ↓
WhisperService (STT) → Transcription
    ↓
ConversationHistory (Context)
    ↓
LLMAction (Chat/Grammar/Vocab/Practice)
    ↓
OllamaService (LLM) → Response Text
    ↓
AllTalkService (TTS) → Audio Clip
    ↓
NPCView (Display subtitle + Play audio)
```

---

## 💡 Usage

### Basic Conversation

1. Look at the NPC in VR
2. Point controller at "🎤 TALK" button and pull trigger (or press A button)
3. Speak your question/sentence
4. Press button again to stop recording
5. Wait for NPC response (subtitle + voice)

### Switching Modes

To change from Chat to Grammar/Vocabulary/Practice mode:

1. **In Unity Editor**: Select NPC → NPCController → Action Mode dropdown
2. **At Runtime**: Call `npcController.SetActionMode(ActionMode.GrammarCheck)` via script

### Example Interactions

**Chat Mode:**
- User: "Tell me about Paris"
- NPC: "Paris is the capital of France, known for..."

**Grammar Mode:**
- User: "I goed to the store yesterday"
- NPC: "The correct form is 'I went to the store.' The verb 'go' is irregular..."

**Vocabulary Mode:**
- User: "What does 'serendipity' mean?"
- NPC: "Serendipity means finding something good without looking for it..."

**Practice Mode:**
- NPC: "Let's practice ordering at a restaurant. I'll be the waiter..."

---

## 📚 Additional Resources

- [Unity XR Documentation](https://docs.unity3d.com/Manual/XR.html)
- [Meta Quest Developer Center](https://developer.oculus.com/)
- [Whisper.Unity GitHub](https://github.com/Macoron/whisper.unity)
- [Ollama Documentation](https://ollama.ai/docs)
- [AllTalk TTS GitHub](https://github.com/erew123/alltalk_tts)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
