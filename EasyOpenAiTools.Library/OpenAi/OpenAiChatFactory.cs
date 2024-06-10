using Microsoft.Extensions.Logging;

namespace EasyOpenAiTools.Library.OpenAi
{
    public class OpenAiChatFactory
    {
        public static OpenAiChat StartNewChat(OpenAiSettings settings, ILogger? logger = null)
        {
            var model = new OpenAiModel(settings, logger);
            return new OpenAiChat(model, settings.InitialPrompt);
        }
    }
}
