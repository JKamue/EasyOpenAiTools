using Microsoft.Extensions.Logging;

namespace EasyOpenAiTools.Library.OpenAi
{
    /// <summary>
    /// The <c>OpenAiChatFactory</c> class provides a factory method for starting new <see cref="OpenAiChat"/>s.
    /// </summary>
    public class OpenAiChatFactory
    {
        /// <summary>
        /// Starts a new chat session with the specified OpenAI settings.
        /// </summary>
        /// <param name="settings">The OpenAI settings to use for the chat.</param>
        /// <param name="logger">Optional logger to be used for logging chat activity. If not provided, logging will be disabled.</param>
        /// <returns>An instance of <see cref="OpenAiChat"/> initialized with the provided settings and ready to be used for asking questions.</returns>
        public static OpenAiChat StartNewChat(OpenAiSettings settings, ILogger? logger = null)
        {
            var model = new OpenAiModel(settings, logger);
            return new OpenAiChat(model, settings.InitialPrompt);
        }
    }
}
