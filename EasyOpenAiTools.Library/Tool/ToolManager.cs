using CSharpFunctionalExtensions;
using EasyOpenAiTools.Library.Tool.Attributes;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Text.Json;

namespace EasyOpenAiTools.Library.Tool
{
    internal class ToolManager
    {
        private List<Tool> toolList = new List<Tool>();

        internal ToolManager(ILogger? logger = null)
        {
            var tools = GetTypesWith<ToolAttribute>();
            logger?.Log(LogLevel.Debug, "{Count} tools found, will try to add them", tools.Count());

            foreach (var tool in tools)
            {
                var registrationResult = Register(tool);

                if (registrationResult.IsSuccess)
                    logger?.Log(LogLevel.Debug, "{Tool} successfully add as tool", tool.FullName);
                else
                    logger?.Log(LogLevel.Warning, "{Tool} could not be added because '{Error}'", tool.FullName, registrationResult.Error);
            }
        }

        // Code by Roger Hill https://stackoverflow.com/a/46965514
        private static IEnumerable<Type> GetTypesWith<T>() where T : Attribute
        {
            var output = new List<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var assembly_types = assembly.GetTypes();

                foreach (var type in assembly_types)
                {
                    if (type.IsDefined(typeof(T), false))
                        output.Add(type);
                }
            }

            return output;
        }

        private Result Register(Type toolType)
        {
            var toolCreationResult = Tool.LoadFrom(toolType);

            if (toolCreationResult.IsSuccess)
                toolList.Add(toolCreationResult.Value);

            return toolCreationResult;
        }

        internal async Task<string>? ExecuteToolByName(string functionName, JsonDocument arguments)
        {
            var toolOrNull = toolList.FirstOrDefault(t => t.ToolAttribute.Name == functionName);

            if (toolOrNull is null)
                return null;

            var tool = toolOrNull;
            var argumentDict = JsonDocumentToDictionary(arguments);

            return await tool.Execute(argumentDict);
        }

        private Dictionary<string, string> JsonDocumentToDictionary(JsonDocument document)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                dictionary.Add(property.Name, property.Value.GetString() ?? string.Empty);
            }

            return dictionary;
        }

        internal List<ChatTool> GetChatTools()
        {
            return toolList.Select(tool => tool.ToChatTool()).ToList();
        }
    }
}
