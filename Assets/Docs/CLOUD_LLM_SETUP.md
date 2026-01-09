# Cloud LLM Setup Guide

## Overview
The Language Tutor now supports multiple LLM providers, including cloud-based services like Google Gemini. This allows you to choose between local (Ollama) and cloud-based AI models.

## Supported Providers

### 1. **Ollama** (Local)
- Runs on your local machine
- No API key required
- No internet dependency
- Free to use
- Models: llama3, mistral, etc.

### 2. **Gemini** (Cloud)
- Google's AI service
- Requires API key
- Internet connection required
- Free tier available
- Models: gemini-pro, gemini-1.5-pro, etc.

### 3. **OpenAI** (Coming Soon)
- GPT-4, GPT-3.5, etc.
- Not yet implemented

### 4. **Azure** (Coming Soon)
- Azure OpenAI Service
- Not yet implemented

## Setup Instructions

### Getting a Gemini API Key

1. **Visit Google AI Studio**
   - Go to [https://makersuite.google.com/app/apikey](https://makersuite.google.com/app/apikey)
   - Sign in with your Google account

2. **Create an API Key**
   - Click "Create API Key"
   - Select an existing Google Cloud project or create a new one
   - Copy the generated API key

3. **Store Safely**
   - Keep your API key secure
   - Don't share it publicly
   - Don't commit it to version control

### Configuring Your LLMConfig

1. **Open or Create LLMConfig**
   - In Unity, go to `Assets -> Create -> Language Tutor -> LLM Config`
   - Or open your existing LLMConfig asset

2. **Set Provider**
   - In the Inspector, find "Provider Selection"
   - Choose your desired provider from the dropdown:
     - `Ollama` - Local AI (default)
     - `Gemini` - Google Gemini
     - `OpenAI` - (Coming soon)
     - `Azure` - (Coming soon)

3. **Configure for Gemini**
   ```
   Provider: Gemini
   Service URL: (not used for Gemini)
   Endpoint Path: (not used for Gemini)
   Model Name: gemini-pro
   API Key: [paste your API key here]
   ```

4. **Configure for Ollama**
   ```
   Provider: Ollama
   Service URL: http://127.0.0.1:11434
   Endpoint Path: /api/generate
   Model Name: llama3
   API Key: (leave empty)
   ```

## Usage Examples

### Example 1: Using Gemini for Language Learning

1. Set up your LLMConfig for Gemini
2. Assign it to your NPCController
3. The system will automatically use Gemini for all AI interactions

### Example 2: Switching Between Providers

You can create multiple LLMConfig assets:
- `LLMConfig_Ollama.asset` - For local testing
- `LLMConfig_Gemini.asset` - For production/cloud

Then switch between them by changing which config is assigned to your NPCController.

## API Key Security Best Practices

### ⚠️ Important Security Notes

1. **Never commit API keys to Git**
   - Add `*.asset` files with API keys to `.gitignore` if they contain sensitive data
   - Or use a separate config file not tracked by Git

2. **Use Environment Variables (Advanced)**
   - For production builds, consider loading API keys from environment variables
   - Implement a custom `LLMConfigLoader` script to read from environment

3. **Rotate Keys Regularly**
   - Change your API keys periodically
   - Revoke old keys in the Google AI Studio

4. **Monitor Usage**
   - Check your Google Cloud Console for API usage
   - Set up billing alerts to avoid unexpected charges

## Troubleshooting

### "API key is required for Gemini service"
- Make sure you've entered your API key in the LLMConfig
- Check that there are no extra spaces or line breaks

### "Request failed" with Gemini
- Verify your API key is valid
- Check your internet connection
- Ensure you haven't exceeded your API quota
- Check the Unity Console for detailed error messages

### Slow Response Times
- Cloud services depend on internet speed
- Consider using local Ollama for development/testing
- Switch to cloud for production when needed

## Model Recommendations

### For Gemini
- **gemini-pro** - Good balance of speed and quality
- **gemini-1.5-pro** - More capable, but slower and more expensive
- **gemini-1.5-flash** - Faster responses, good for real-time interaction

### For Ollama
- **llama3** - Excellent general purpose model
- **mistral** - Fast and efficient
- **phi3** - Lightweight, good for lower-end hardware

## Cost Considerations

### Gemini Pricing (as of 2024)
- Free tier: 60 requests per minute
- Paid tier: Pay per 1000 tokens
- Check [Google AI Pricing](https://ai.google.dev/pricing) for current rates

### Ollama
- Completely free
- Only requires local compute resources
- No API limits or quotas

## Next Steps

1. Get your API key from Google AI Studio
2. Configure your LLMConfig
3. Test with a simple conversation
4. Monitor your API usage
5. Adjust temperature and token limits for optimal results

## Support

For issues or questions:
- Check Unity Console for detailed error logs
- Review the [Gemini API Documentation](https://ai.google.dev/docs)
- Check the project's GitHub issues

---

**Note**: This system is designed to be extensible. Additional providers (OpenAI, Azure, Anthropic) can be added by creating new service classes that implement `ILLMService`.
