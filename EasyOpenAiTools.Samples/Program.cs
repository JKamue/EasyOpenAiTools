using EasyOpenAiTools.Library.OpenAi;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace EasyOpenAiTools.Samples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // This is only needed if you want to log to console
            // Make sure to also use Microsoft.Extensions.Logging.Console for this to work
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug);
            });
            var logger = loggerFactory.CreateLogger("EasyOpenAi");

            var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var openAiModelType = OpenAiModelType.Gpt4;
            var initialPrompt = """
                Answer all questions by users in a brief and concise way.
                """;

            var openAiSettings = new OpenAiSettings(openAiApiKey, openAiModelType, initialPrompt);
            var chat = OpenAiChatFactory.StartNewChat(openAiSettings, logger);
            
            Console.WriteLine("LLM chat (write exit to close): ");

            var userMessage = String.Empty;
            while (userMessage != "exit")
            {
                Console.Write("[User]: ");
                userMessage = Console.ReadLine();

                var response = chat.Ask(userMessage).Result;                

                Console.WriteLine($"[Assistant]: {response}");
            }
        }
    }
}
