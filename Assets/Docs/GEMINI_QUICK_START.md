# Quick Start: Using Gemini API

## 3 Simple Steps to Get Started

### Step 1: Get Your API Key (2 minutes)
1. Go to https://makersuite.google.com/app/apikey
2. Sign in with Google
3. Click "Create API Key"
4. Copy your key

### Step 2: Configure Unity (1 minute)
1. Open your LLMConfig asset in Unity Inspector
2. Set these fields:
   - **Provider**: Choose `Gemini`
   - **Model Name**: `gemini-pro`
   - **API Key**: Paste your key from Step 1

### Step 3: Test It
1. Run your scene
2. The system will automatically use Gemini
3. Check Console for "[GeminiService] Sending request to Gemini API"

## Example Configuration

```
┌─────────────────────────────────────┐
│ LLM Config (Inspector)              │
├─────────────────────────────────────┤
│ Provider Selection:                 │
│   Provider: Gemini          ▼       │
│                                     │
│ Service Configuration:              │
│   Service URL: (ignored)            │
│   Endpoint Path: (ignored)          │
│   Model Name: gemini-pro            │
│                                     │
│ Authentication:                     │
│   API Key: AIzaSyD...              │
│   (paste your key here)             │
│                                     │
│ Request Settings:                   │
│   Max Tokens: 512                   │
│   Temperature: 0.7                  │
└─────────────────────────────────────┘
```

## Switching Back to Local Ollama

Simply change the Provider dropdown back to `Ollama` - that's it!

## Need Help?

See [CLOUD_LLM_SETUP.md](CLOUD_LLM_SETUP.md) for detailed instructions, troubleshooting, and security best practices.
