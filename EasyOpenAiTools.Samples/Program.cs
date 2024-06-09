using EasyOpenAiTools.Library.OpenAi;
using OpenAI.Chat;

namespace EasyOpenAiTools.Samples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var openAiModelType = OpenAiModelType.Gpt4;
            var initialPrompt = """
                Answer all questions by users in a brief and concise way.
                """;


            var openAiModelSettings = new OpenAiModelSettings(openAiApiKey, openAiModelType, initialPrompt);
            var openAiModel = new OpenAiModel(openAiModelSettings);

            Console.WriteLine("LLM chat (write exit to close): ");

            var thread = new List<ChatMessage>();
            var userMessage = String.Empty;
            while (userMessage != "exit")
            {
                Console.Write("[User]: ");
                userMessage = Console.ReadLine();

                if (thread.Count == 0)
                {
                    thread = openAiModel.CreateThread(userMessage).Result;
                }
                else
                {
                    thread = openAiModel.AskInThread(userMessage, thread).Result;
                }

                var response = thread.Last();
                Console.WriteLine($"[Assistant]: {response.Content[0].Text}");
            }
        }
    }
}
