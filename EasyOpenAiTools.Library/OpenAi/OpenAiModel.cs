using CSharpFunctionalExtensions;
using EasyOpenAiTools.Library.Tool;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Text.Json;

namespace EasyOpenAiTools.Library.OpenAi
{
    internal class OpenAiModel
    {
        private readonly ChatClient _client;
        private readonly ToolManager _toolManager;
        private readonly ILogger? _logger;

        internal OpenAiModel(OpenAiSettings settings, ILogger? logger = null)
        {
            _client = new ChatClient(
                settings.OpenAiModel,
                settings.OpenAiApiKey
            );

            _toolManager = new ToolManager(logger);
            _logger = logger;
        }

        internal async Task<Result<List<ChatMessage>>> Ask(string question, List<ChatMessage> previousChat)
        {
            var message = new UserChatMessage(question);

            previousChat.Add(message);

            return await ExecuteQuestion(previousChat);
        }

        private ChatCompletionOptions GenerateChatCompletionOptions()
        {
            var chatTools = _toolManager.GetChatTools();
            var chatCompletionOptions = new ChatCompletionOptions();

            foreach (var tool in chatTools)
                chatCompletionOptions.Tools.Add(tool);

            return chatCompletionOptions;
        }

        private async Task<Result<List<ChatMessage>>> ExecuteQuestion(List<ChatMessage> messages)
        {
            ChatCompletion chatCompletion = _client.CompleteChat(messages, GenerateChatCompletionOptions());

            if (chatCompletion.FinishReason == ChatFinishReason.Length)
                return Result.Failure<List<ChatMessage>>("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

            if (chatCompletion.FinishReason == ChatFinishReason.ContentFilter)
                return Result.Failure<List<ChatMessage>>("Omitted content due to a content filter flag.");

            if (chatCompletion.FinishReason == ChatFinishReason.FunctionCall)
                return Result.Failure<List<ChatMessage>>("Deprecated function call attempted.");

            messages.Add(new AssistantChatMessage(chatCompletion));

            if (chatCompletion.FinishReason == ChatFinishReason.ToolCalls)
            {
                var toolCallResults = await RunToolCalls(chatCompletion.ToolCalls);

                messages.AddRange(toolCallResults);

                // Run the Model again with the new information added
                var messageResult = await ExecuteQuestion(messages);
                if (messageResult.IsFailure)
                    return messageResult;

                messages = messageResult.Value;
            }

            return messages;
        }

        private async Task<List<ChatMessage>> RunToolCalls(IReadOnlyList<ChatToolCall> toolCalls)
        {
            var newMessages = new List<ChatMessage>();

            foreach (var toolCall in toolCalls)
                newMessages.Add(await RunToolCall(toolCall));

            return newMessages;
        }

        private async Task<ChatMessage> RunToolCall(ChatToolCall toolCall)
        {
            _logger.Log(LogLevel.Debug, "Executing {Tool} with arguments '{Arguments}'", toolCall.FunctionName, toolCall.FunctionArguments);
            using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
            var toolResult = await _toolManager.ExecuteToolByName(toolCall.FunctionName, argumentsJson)
                ?? "This Tool does not exist! If it is a calculation you can do yourself then do it yourself without telling the user. Else tell him that you currently cannot answer the question";
            return new ToolChatMessage(toolCall.Id, toolResult);
        }
    }
}
