using OpenAI.Chat;

namespace EasyOpenAiTools.Library.OpenAi
{
    public class OpenAiChat
    {
        private readonly OpenAiModel _model;
        private List<ChatMessage> messageLog = new();

        internal OpenAiChat(OpenAiModel model, string initialPrompt)
        {
            _model = model;
            messageLog.Add(CreateInitialMessage(initialPrompt));
        }

        private SystemChatMessage CreateInitialMessage(string initialPrompt)
        {
            return new SystemChatMessage(initialPrompt);
        }

        public async Task<string> Ask(string message)
        {
            messageLog = await _model.Ask(message, messageLog);
            return messageLog.Last().Content.First().Text;
        }
    }
}