using System;
using System.Threading.Tasks;
using LanguageTutor.Services;

namespace LanguageTutor.Actions
{
    /// <summary>
    /// Grammar correction action for language learning.
    /// Analyzes user input for grammatical errors and provides corrections.
    /// </summary>
    public class GrammarCheckAction : ILLMAction
    {
        private readonly string _targetLanguage;
        private const string DEFAULT_GRAMMAR_PROMPT = 
            @"You are a language tutor focused on grammar correction. The user is learning {0}.

Analyze the following text for grammatical errors:
'{1}'

If there are errors:
1. Provide the corrected version
2. Explain what was wrong
3. Be encouraging and constructive

If there are no errors, praise the user and confirm the grammar is correct.

Keep your response concise and clear.";

        public GrammarCheckAction(string targetLanguage = "English")
        {
            _targetLanguage = targetLanguage;
        }

        public string GetActionName() => "GrammarCheck";

        public bool CanExecute(LLMActionContext context)
        {
            return !string.IsNullOrWhiteSpace(context?.UserInput);
        }

        public async Task<LLMActionResult> ExecuteAsync(ILLMService llmService, LLMActionContext context)
        {
            try
            {
                string language = !string.IsNullOrEmpty(context.TargetLanguage) 
                    ? context.TargetLanguage 
                    : _targetLanguage;

                string prompt = string.Format(DEFAULT_GRAMMAR_PROMPT, language, context.UserInput);

                // Use the system prompt from context if provided, otherwise use our constructed prompt
                string response = await llmService.GenerateResponseAsync(
                    context.UserInput, 
                    prompt, 
                    context.ConversationHistory);

                var result = LLMActionResult.CreateSuccess(response);
                result.Metadata["original_text"] = context.UserInput;
                result.Metadata["target_language"] = language;

                return result;
            }
            catch (Exception ex)
            {
                return LLMActionResult.CreateFailure($"Grammar check failed: {ex.Message}");
            }
        }
    }
}
