# Enhanced NPC Setup - Add Character & Beautiful UI

## 🎯 VR/AR HUD - Always Visible Display

### Quick Overview
For AR/VR applications, we'll create a **World Space Canvas** that follows your view and displays conversation information persistently. This HUD will:
- Float in front of you at comfortable viewing distance (1.5m)
- Show subtitles, status, and controls
- Follow your head movement smoothly
- Work in both AR and VR modes

---

## 🔹 VR HUD Setup (30 minutes)

### Step 1: Create World Space Canvas

1. **Hierarchy** → Right-click → **UI** → **Canvas**
2. Rename to `VRHudCanvas`
3. **Inspector** settings:
   ```
   Canvas:
   - Render Mode: World Space
   - Event Camera: [Assign Main Camera or XR Rig Camera]
   
   Rect Transform:
   - Position: X: 0, Y: 1.6, Z: 1.5 (in front of eyes, 1.5m away)
   - Width: 800
   - Height: 400
   - Scale: X: 0.001, Y: 0.001, Z: 0.001 (makes it readable size)
   ```

### Step 2: Make HUD Follow Camera (Quest 3 Method)

**For Meta Quest 3 - No Script Needed!**

Simply parent the Canvas to your camera:

1. **Find your camera** in Hierarchy:
   - If using OVRCameraRig: Look for `OVRCameraRig → TrackingSpace → CenterEyeAnchor`
   - If using basic XR: Look for `Main Camera`

2. **Drag VRHudCanvas** onto the CenterEyeAnchor (or Main Camera) to make it a child

3. **Set local position** on VRHudCanvas:
   ```
   Rect Transform (Local):
   - Position: X: 0, Y: -0.2, Z: 1.5
   - Rotation: X: 0, Y: 0, Z: 0
   - Scale: X: 0.001, Y: 0.001, Z: 0.001
   ```

Done! The HUD now follows your head perfectly with zero scripting.

**Alternative: Use OVROverlay (Best Performance)**

For even better performance on Quest 3:

1. Add Component to VRHudCanvas → **OVR Overlay**
2. Settings:
   - Current Overlay Type: Overlay
   - Composition Depth: 1
   - Texture: [Auto-generated from Canvas]
   
This renders the UI on a separate layer, saving GPU power!

### Step 3: Design VR HUD Layout

Inside `VRHudCanvas`, create:

#### A) Background Panel (Optional, semi-transparent)
1. Right-click VRHudCanvas → UI → Image
2. Rename: `HudBackground`
3. Settings:
   ```
   Rect Transform:
   - Anchor: Stretch/Stretch
   - Margins: 20 all sides
   
   Image:
   - Color: RGBA(0, 0, 0, 100) - very subtle dark
   - Raycast Target: OFF (don't block interactions)
   ```

#### B) Subtitle Display (Top Center)
1. Right-click VRHudCanvas → UI → TextMeshPro
2. Rename: `SubtitleText`
3. Settings:
   ```
   Rect Transform:
   - Anchor: Top Center
   - Position: X: 0, Y: -50
   - Width: 700, Height: 120
   
   TextMeshPro:
   - Font Size: 28
   - Alignment: Center
   - Color: White
   - Enable Word Wrapping
   - Overflow: Ellipsis
   ```

#### C) Status Panel (Top Left Corner)
1. Right-click VRHudCanvas → UI → Image
2. Rename: `StatusPanel`
3. Settings:
   ```
   Rect Transform:
   - Anchor: Top Left
   - Position: X: 50, Y: -30
   - Width: 200, Height: 60
   
   Image:
   - Color: RGBA(20, 20, 20, 180)
   - Sprite: UI/UISprite (rounded)
   ```

4. Inside StatusPanel, add TextMeshPro:
   - Rename: `StatusText`
   - Text: "● Ready"
   - Font Size: 20
   - Color: Green
   - Alignment: Middle Left

#### D) Mode Indicator (Top Right)
1. Right-click VRHudCanvas → UI → Image
2. Rename: `ModePanel`
3. Settings similar to StatusPanel
4. Add TextMeshPro inside: `ModeText` → "Chat Mode"

#### E) Interaction Prompt (Bottom Center)
1. Right-click VRHudCanvas → UI → Image
2. Rename: `InteractionPrompt`
3. Settings:
   ```
   Rect Transform:
   - Anchor: Bottom Center
   - Position: X: 0, Y: 30
   - Width: 300, Height: 60
   
   Image:
   - Color: RGBA(0, 120, 215, 200) - Blue
   - Sprite: UI/UISprite
   ```

4. Add TextMeshPro inside:
   - Rename: `PromptText`
   - Text: "🎤 Press to Talk"
   - Font Size: 24
   - Alignment: Center

5. Add Button component to InteractionPrompt:
   - Navigation: None
   - Transition: Color Tint
   - This will be your Talk Button

#### F) Visual Feedback Indicator (Center)
1. Right-click VRHudCanvas → UI → Image
2. Rename: `RecordingIndicator`
3. Settings:
   ```
   Rect Transform:
   - Anchor: Center
   - Position: X: 0, Y: -120
   - Width: 80, Height: 80
   
   Image:
   - Color: Red
   - Sprite: UI/UISprite (circle)
   - Preserve Aspect: ON
   ```

4. Add **Animator** or pulse script for recording feedback

### Step 4: Update NPCView Script

Assign your new VR HUD elements to NPCView:

1. **Select your NPCSystem** (or character with NPCController)
2. Find **NPCView component** in Inspector
3. **Assign UI Components**:
   - Subtitle Text → `SubtitleText` (from VRHudCanvas)
   - Talk Button → `InteractionPrompt` (Button component)
   - Status Indicator → `RecordingIndicator`

### Step 5: Quest 3 Hand Tracking / Controller Input

**Option 1: Use Quest Controllers (Easiest)**
- The Button component on InteractionPrompt already works with Quest controller raycasting
- Just point your controller laser at the "Press to Talk" button and pull trigger
- OVRCameraRig handles raycasting automatically

**Option 2: Add Hand Tracking Buttons**
1. Add Component to InteractionPrompt → **OVRGrabbable** or **OVRTouchable**
2. User can poke the button with finger tracking

**Option 3: Voice Command or Controller Button**
Add this to AudioInputController.cs in the Update() method:

```csharp
// Add using statement at top: using UnityEngine.XR;

// In Update(), add:
OVRInput.Update();
if (OVRInput.GetDown(OVRInput.Button.One)) // A button on right controller
{
    if (!isRecording)
        StartRecording();
    else
        StopRecording();
}
```

### Step 6: XR Optimization

For Quest 3 performance:

1. **Canvas** settings:
   ```
   Additional Shader Channels: Everything
   Pixel Perfect: OFF (for VR)
   ```

2. **Text Quality**:
   - Select all TextMeshPro components
   - Enable: Extra Padding
   - Render Mode: DistanceField (better for VR)

---

## 📐 VR HUD Design Best Practices

### Positioning Guidelines
- **Distance**: 1.2m - 2.0m from eyes (comfort zone)
- **Height**: Slightly below eye level (-0.2 to 0.2 from camera)
- **Size**: Keep UI readable but not overwhelming (800x400 canvas at 0.001 scale works well)

### Color Recommendations
- **Background**: Very dark semi-transparent (don't block real world in AR)
- **Text**: High contrast - White or Cyan on dark background
- **Status**: Green (Ready), Yellow (Processing), Red (Error/Recording)
- **Accents**: Blue for interactive elements

### Performance Tips
- **Limit Canvas rebuilds**: Use separate Canvas for frequently updating text
- **Reduce overdraw**: Minimize overlapping UI elements
- **Optimize text**: Use TextMeshPro with SDF rendering
- **Pooling**: If showing/hiding elements frequently, use object pooling

### Accessibility
- **Font Size**: Minimum 20pt for VR readability
- **Contrast**: At least 4.5:1 ratio
- **Animation**: Smooth, not jarring (use smoothSpeed 5-10)
- **Feedback**: Clear visual confirmation for all actions

---

## Part 1: Adding Your NPC Character to the Scene

### Step 1: Choose Your Character

I can see you have **PartyPeople** character prefabs available. Let's add one!

1. **In Project window**, navigate to:
   ```
   Assets → Assets → PartyPeople → Pack_FREE_PartyCharacters → Prefabs
   ```

2. **Choose a character** (any of the Character_XXXXXX prefabs)
   - You already have `Character_0413da` - we can use that or pick a new one
   - Recommended: Try different ones to see which looks best!

3. **Drag the prefab** into your Scene view or Hierarchy

### Step 2: Position the NPC

1. **Select the character** in Hierarchy

2. **In Inspector**, set Transform:
   ```
   Position: X: 0, Y: 0, Z: 3 (character in front of camera)
   Rotation: X: 0, Y: 180, Z: 0 (facing camera)
   Scale: X: 1, Y: 1, Z: 1
   ```

3. **Adjust camera to frame the character nicely:**
   - Select Main Camera
   - Position: `X: 0, Y: 1.5, Z: 0` (at head height)
   - Look at the character's upper body/face

### Step 3: Attach NPCSystem to the Character

**Option 1: Add Controller to Character (Recommended)**

1. **Select your character** GameObject in Hierarchy
2. **Add Component** → `LanguageTutor.Core.NPCController`
3. **Assign all the configs** (same as before):
   - DefaultLLMConfig
   - DefaultTTSConfig
   - DefaultSTTConfig
   - DefaultConversationConfig
   - WhisperManager
   - NPCView (we'll create better UI next)

**Option 2: Keep Separate NPCSystem**

- Keep your empty `NPCSystem` GameObject
- Just position the character for visual purposes

---

## Part 2: Creating a Beautiful UI

### Design 1: Modern Language Tutor UI

Let's create a clean, professional interface!

#### Step 1: Clear Old UI

1. **Delete** the old basic UI elements (old SubtitleText, TalkButton if too simple)
2. Keep the Canvas and NPCView GameObject

#### Step 2: Create Background Panel

1. **Right-click Canvas** → **UI** → **Panel**
2. **Name it**: `UIBackground`
3. **Position**: Bottom of screen
   - Anchor: Bottom-stretch (hold Alt+Shift, click bottom-center)
   - Height: `200`
   - Color: Semi-transparent dark `RGBA: 0, 0, 0, 150`

#### Step 3: Create New Subtitle Display

1. **Right-click UIBackground** → **UI** → **Panel**
2. **Name it**: `SubtitlePanel`
3. **Settings**:
   ```
   Anchor: Center-stretch (full width)
   Height: 120
   Pos Y: 40
   Color: RGBA: 20, 20, 30, 200 (dark blue-gray)
   ```

4. **Add subtle border:**
   - With SubtitlePanel selected
   - Add Component → **Outline**
   - Effect Color: White or cyan
   - Effect Distance: X: 0, Y: -2

5. **Create text inside:**
   - Right-click SubtitlePanel → **UI** → **Text - TextMeshPro**
   - Name it: `SubtitleText`
   - Settings:
     ```
     Anchor: Stretch all (fill parent)
     Margins: Left: 20, Right: 20, Top: 10, Bottom: 10
     Font Size: 20
     Alignment: Center-Middle
     Color: White
     Enable Word Wrapping: ✓
     Overflow: Truncate (or Ellipsis)
     ```

6. **Drag SubtitleText** to NPCView's **Subtitle Text** field

#### Step 4: Create Stylish Talk Button

1. **Right-click UIBackground** → **UI** → **Button - TextMeshPro**
2. **Name it**: `TalkButton`
3. **Position**:
   ```
   Anchor: Bottom-center
   Pos Y: -60
   Width: 180
   Height: 70
   ```

4. **Style the button:**
   - **Select TalkButton**
   - Image Color: `RGBA: 50, 150, 255, 255` (nice blue)
   - Add Component → **Shadow**
     - Effect Distance: X: 2, Y: -2
     - Effect Color: Black, Alpha: 150

5. **Style button text** (select child Text object):
   ```
   Text: "🎤 TALK"
   Font Size: 28
   Font Style: Bold
   Color: White
   ```

6. **Add hover effect** (optional but nice):
   - Select TalkButton
   - In Button component, expand **Colors**
   - Normal: Your blue color
   - Highlighted: Lighter blue `RGBA: 80, 180, 255, 255`
   - Pressed: Darker blue `RGBA: 30, 120, 200, 255`
   - Transition: Color Tint (with duration 0.1)

7. **Drag TalkButton** to NPCView's **Talk Button** field

#### Step 5: Create Status Indicator (Nice Design)

1. **Right-click Canvas** → **UI** → **Image**
2. **Name it**: `StatusIndicator`
3. **Position**: Top-right
   ```
   Anchor: Top-right
   Pos X: -40
   Pos Y: -40
   Width: 50
   Height: 50
   ```

4. **Make it circular:**
   - In Image component, change **Image Type** to: `Filled`
   - Fill Method: `Radial 360`
   - Or use a circle sprite if you have one

5. **Add pulsing effect** (optional - we can script this):
   - Color: White (will change based on state)

6. **Drag StatusIndicator** to NPCView's **Status Indicator** field

#### Step 6: Add Mode Selection Buttons (Optional - Advanced)

Create buttons to switch between Chat, Grammar Check, Vocabulary modes:

1. **Right-click UIBackground** → **UI** → **Panel**
2. **Name it**: `ModePanel`
3. **Position**: Top of UIBackground
   ```
   Anchor: Top-stretch
   Height: 40
   Pos Y: 0
   ```

4. **Add Horizontal Layout:**
   - Select ModePanel
   - Add Component → **Horizontal Layout Group**
   - Settings:
     ```
     Padding: All sides = 10
     Spacing: 10
     Child Alignment: Middle Center
     Child Force Expand: Width ✓
     ```

5. **Create mode buttons** (repeat 4 times):
   - Right-click ModePanel → **UI** → **Button - TextMeshPro**
   - Names: `ChatButton`, `GrammarButton`, `VocabButton`, `PracticeButton`
   - Each button text: "Chat", "Grammar", "Vocab", "Practice"
   - Font size: 16
   - Colors: Different color per mode (blue, green, purple, orange)

#### Step 7: Add Conversation History Panel (Optional - Advanced)

1. **Right-click Canvas** → **UI** → **Panel**
2. **Name it**: `HistoryPanel`
3. **Position**: Left side
   ```
   Anchor: Left-stretch
   Pos X: 0
   Width: 300y
   ```
4. **Add Scroll View** inside for scrolling conversation history

---

## Part 3: Enhanced Visual Effects

### Add Particle Effects (Optional)

When NPC is speaking, add particles:

1. **Right-click character** → **Effects** → **Particle System**
2. **Name it**: `SpeakingParticles`
3. **Position**: Near character's mouth
4. **Settings**:
   ```
   Start Lifetime: 0.5
   Start Speed: 0.5
   Start Size: 0.1
   Emission Rate: 20
   Shape: Sphere, Radius: 0.2
   Color over Lifetime: White → Transparent
   ```
5. **Disable** "Play On Awake"
6. **Play it** when NPC speaks (we can script this)

### Add Spotlight on NPC

1. **Right-click in Hierarchy** → **Light** → **Spot Light**
2. **Name it**: `NPCSpotlight`
3. **Position**: Above and in front of character
   ```
   Position: X: 0, Y: 3, Z: 1
   Rotation: X: 60, Y: 0, Z: 0
   ```
4. **Settings**:
   ```
   Color: Warm white or cyan
   Intensity: 3
   Range: 5
   Spot Angle: 60
   ```

---

## Part 4: Scripting UI Enhancements

### Create ModeSelector Script

To switch between Chat/Grammar/Vocab modes with your new buttons:

1. **Create new script**: `Assets/_Project/Scripts/UI/ModeSelector.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;
using LanguageTutor.Core;

namespace LanguageTutor.UI
{
    public class ModeSelector : MonoBehaviour
    {
        [Header("References")]
        public NPCController npcController;
        
        [Header("Mode Buttons")]
        public Button chatButton;
        public Button grammarButton;
        public Button vocabButton;
        public Button practiceButton;
        
        [Header("Button Colors")]
        public Color activeColor = new Color(0.2f, 0.8f, 1f);
        public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f);
        
        private void Start()
        {
            // Setup button listeners
            chatButton?.onClick.AddListener(() => SetMode(ActionMode.Chat));
            grammarButton?.onClick.AddListener(() => SetMode(ActionMode.GrammarCheck));
            vocabButton?.onClick.AddListener(() => SetMode(ActionMode.VocabularyTeach));
            practiceButton?.onClick.AddListener(() => SetMode(ActionMode.ConversationPractice));
            
            // Set initial mode
            SetMode(ActionMode.Chat);
        }
        
        private void SetMode(ActionMode mode)
        {
            npcController.SetActionMode(mode);
            UpdateButtonColors(mode);
        }
        
        private void UpdateButtonColors(ActionMode activeMode)
        {
            // Reset all to inactive
            SetButtonColor(chatButton, activeMode == ActionMode.Chat ? activeColor : inactiveColor);
            SetButtonColor(grammarButton, activeMode == ActionMode.GrammarCheck ? activeColor : inactiveColor);
            SetButtonColor(vocabButton, activeMode == ActionMode.VocabularyTeach ? activeColor : inactiveColor);
            SetButtonColor(practiceButton, activeMode == ActionMode.ConversationPractice ? activeColor : inactiveColor);
        }
        
        private void SetButtonColor(Button button, Color color)
        {
            if (button != null)
            {
                var colors = button.colors;
                colors.normalColor = color;
                colors.selectedColor = color;
                button.colors = colors;
            }
        }
    }
}
```

2. **Add to scene:**
   - Select ModePanel (or create empty GameObject)
   - Add Component → ModeSelector
   - Assign NPCController and all buttons

---

## Part 5: Final Polish

### Add Background

1. **Create a simple backdrop** behind the character:
   - Right-click Hierarchy → **3D Object** → **Quad**
   - Name: `Background`
   - Position: Behind character `Z: 5`
   - Scale: `X: 10, Y: 6`
   - Add a nice gradient texture or solid color

### Add Ambient Animation

Make the NPC character animate while idle:

1. **Select character** GameObject
2. If it has an **Animator** component:
   - Assign an idle animation clip
   - Or create a simple animation:
     - Window → Animation → Animation
     - Create New Clip: "Idle"
     - Add property: Transform.Position.Y
     - Keyframe: 0s = 0, 1s = 0.05, 2s = 0
     - Makes character gently bob up and down

### Audio Visualization (Advanced)

Create a script to visualize audio when NPC speaks:

```csharp
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform[] visualBars; // Array of bars that scale with audio
    
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            // Get audio data
            float[] spectrum = new float[64];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            
            // Scale bars based on spectrum
            for (int i = 0; i < visualBars.Length && i < spectrum.Length; i++)
            {
                float height = spectrum[i] * 50f;
                visualBars[i].localScale = new Vector3(1, height, 1);
            }
        }
    }
}
```

---

## Quick Setup Summary

**Minimum for nice appearance:**

1. ✅ Add character prefab (PartyPeople)
2. ✅ Position character nicely (Z: 3, Y: 0, facing camera)
3. ✅ Create semi-transparent bottom panel (UIBackground)
4. ✅ Styled subtitle panel inside (dark with outline)
5. ✅ Stylish talk button with icon (🎤 TALK)
6. ✅ Circular status indicator (top-right)
7. ✅ Add spotlight on character
8. ✅ Set camera at eye level (Y: 1.5)

**Advanced additions:**
- Mode selection buttons
- Conversation history panel
- Particle effects when speaking
- Audio visualization bars
- Character idle animation

---

## UI Color Schemes

### Option 1: Modern Tech (Cyan/Blue)
```
Background: RGBA: 15, 20, 35, 200
Panel: RGBA: 25, 35, 55, 220
Accent: RGBA: 0, 180, 255, 255
Text: White
```

### Option 2: Warm Learning (Orange/Yellow)
```
Background: RGBA: 40, 30, 20, 200
Panel: RGBA: 60, 45, 30, 220
Accent: RGBA: 255, 160, 50, 255
Text: Cream/Light Yellow
```

### Option 3: Nature (Green)
```
Background: RGBA: 20, 30, 25, 200
Panel: RGBA: 30, 50, 40, 220
Accent: RGBA: 100, 220, 150, 255
Text: White
```

---

## Pro Tips

1. **UI Scale:** Make sure Canvas Scaler is set to "Scale With Screen Size"
   - Reference Resolution: `1920 x 1080`
   - Match: 0.5

2. **Fonts:** Import a nice Google Font for better typography
   - Download from fonts.google.com
   - Import into Unity → Create TextMeshPro Font Asset

3. **Icons:** Use Unicode emojis in text: 🎤 🔊 ✏️ 📚 💬

4. **Animations:** Use DOTween (free asset) for smooth UI transitions

5. **Testing:** Test on different screen sizes/resolutions

---

Your enhanced NPC setup is ready! 🎨✨
