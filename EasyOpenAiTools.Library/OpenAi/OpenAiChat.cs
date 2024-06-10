using CSharpFunctionalExtensions;
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
            var messageResult = await AskWithResult(message);

            if (messageResult.IsFailure)
                return $"Response failed because '{messageResult.Error}'";

            return messageResult.Value;
        }

        public async Task<Result<string>> AskWithResult(string message)
        {
            var messageResult = await _model.Ask(message, messageLog);

            if (messageResult.IsFailure)
                return Result.Failure<string>(messageResult.Error);

            messageLog = messageResult.Value;
            return GetLastMessage();
        }

        public string GetLastMessage() => messageLog.Last().Content.First().Text;
    }
}