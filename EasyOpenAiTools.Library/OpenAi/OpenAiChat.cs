using CSharpFunctionalExtensions;
using OpenAI.Chat;

namespace EasyOpenAiTools.Library.OpenAi
{
    /// <summary>
    /// The <c>OpenAiChat</c> class facilitates interaction with an OpenAI language model by maintaining a chat session.
    /// It allows sending messages to the model and receiving responses.
    /// </summary>
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

        /// <summary>
        /// Sends a message to the OpenAI model and returns the response as a string.
        /// </summary>
        /// <param name="message">The message to send to the model.</param>
        /// <returns>The response from the model as a string. If the response fails, a failure message is contained in the returned string.</returns>
        /// <remarks>
        /// Check out <see cref="AskWithResult(string)"/> that wont embed errors into the resulting string and will instead return 
        /// a result monade that is either a <c>Success</c> or <c>Failure</c>
        /// </remarks>
        public async Task<string> Ask(string message)
        {
            var messageResult = await AskWithResult(message);

            if (messageResult.IsFailure)
                return $"Response failed because '{messageResult.Error}'";

            return messageResult.Value;
        }

        /// <summary>
        /// Sends a message to the OpenAI model and returns the result as a <see cref="Result{string}"/> object.
        /// </summary>
        /// <param name="message">The message to send to the model.</param>
        /// <returns>A <see cref="Result{string}"/> object containing the response from the model. If the response fails, the result contains the error.</returns>
        public async Task<Result<string>> AskWithResult(string message)
        {
            var messageResult = await _model.Ask(message, messageLog);

            if (messageResult.IsFailure)
                return Result.Failure<string>(messageResult.Error);

            messageLog = messageResult.Value;
            return GetLastMessage();
        }

        /// <summary>
        /// Gets the content of the last message in the chat.
        /// </summary>
        /// <returns>The content of the last message as a string.</returns>
        public string GetLastMessage() => messageLog.Last().Content.First().Text;
    }
}