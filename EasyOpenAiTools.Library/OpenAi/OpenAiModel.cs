using EasyOpenAiTools.Library.Tool;
using OpenAI.Chat;
using System.Text.Json;

namespace EasyOpenAiTools.Library.OpenAi
{
    public class OpenAiModel
    {
        private readonly ChatClient _client;
        private readonly SystemChatMessage _modelPrompt;
        private readonly ToolManager _toolManager;

        public OpenAiModel(OpenAiModelSettings settings)
        {
            _client = new ChatClient(
                settings.OpenAiModel,
                settings.OpenAiApiKey
            );

            _modelPrompt = new SystemChatMessage(settings.InitialPrompt);
            _toolManager = new ToolManager();
        }

        public async Task<List<ChatMessage>> CreateThread(string question)
        {
            List<ChatMessage> messages = [
                _modelPrompt,
                new UserChatMessage(question),
            ];

            return await RunThread(messages);
        }

        public async Task<List<ChatMessage>> AskInThread(string question, List<ChatMessage> thread)
        {
            var message = new UserChatMessage(question);

            thread.Add(message);

            return await RunThread(thread);
        }

        private async Task<List<ChatMessage>> RunThread(List<ChatMessage> messages)
        {
            return await ExecuteQuestion(messages);
        }

        private ChatCompletionOptions GenerateChatCompletionOptions()
        {
            var chatTools = _toolManager.GetChatTools();
            var chatCompletionOptions = new ChatCompletionOptions();

            foreach (var tool in chatTools)
                chatCompletionOptions.Tools.Add(tool);

            return chatCompletionOptions;
        }

        private async Task<List<ChatMessage>> ExecuteQuestion(List<ChatMessage> messages)
        {
            bool requiresAction;

            do
            {
                requiresAction = false;
                ChatCompletion chatCompletion = _client.CompleteChat(messages, GenerateChatCompletionOptions());

                switch (chatCompletion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            messages.Add(new AssistantChatMessage(chatCompletion));
                            break;
                        }

                    case ChatFinishReason.ToolCalls:
                        {
                            // First, add the assistant message with tool calls to the conversation history.
                            messages.Add(new AssistantChatMessage(chatCompletion));

                            // Then, add a new tool message for each tool call that is resolved.
                            foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                            {
                                using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                var toolResult = await _toolManager.ExecuteToolByName(toolCall.FunctionName, argumentsJson) ?? "This Tool does not exist! If it is a calculation you can do yourself then do it yourself without telling the user. Else tell him that you currently cannot answer the question";
                                messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                            }

                            requiresAction = true;
                            break;
                        }

                    case ChatFinishReason.Length:
                        throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException("Omitted content due to a content filter flag.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Deprecated in favor of tool calls.");

                    default:
                        throw new NotImplementedException(chatCompletion.FinishReason.ToString());
                }
            } while (requiresAction);

            return messages;
        }
    }
}
