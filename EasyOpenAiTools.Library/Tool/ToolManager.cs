using EasyOpenAiTools.Library.Tool.Attributes;
using OpenAI.Chat;
using System.Reflection;
using System.Text.Json;

namespace EasyOpenAiTools.Library.Tool
{
    internal class ToolManager
    {
        private List<Tool> toolList = new List<Tool>();

        internal ToolManager()
        {
            var tools = GetTypesWith<ToolAttribute>();
            foreach (var tool in tools)
            {
                Register(tool);
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

        private bool Register(Type toolType)
        {
            var toolAttributeOrNull = ReadAttributeOfClass<ToolAttribute>(toolType);
            if (toolAttributeOrNull is null)
                return false;

            var toolAttribute = toolAttributeOrNull;
            var propertiesWithToolAttribute = GetPropertiesWithAttribute<ToolPropertyAttribute>(toolType);

            var toolProperties = propertiesWithToolAttribute
                .Select(propertyWithToolAttribute =>
                    new ToolProperty(
                        propertyWithToolAttribute.property,
                        propertyWithToolAttribute.attribute))
                .ToList();

            var methods = toolType.GetMethods();
            var methodsWithAttribute = methods.Where(m => m.GetCustomAttribute<ToolMethod>() != null).ToArray();

            if (methodsWithAttribute.Count() != 1)
                return false;


            var tool = new Tool(toolType, toolAttribute, toolProperties, methodsWithAttribute.First());

            toolList.Add(tool);

            return true;
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

        private static T? ReadAttributeOfClass<T>(Type type) where T : Attribute
        {
            var toolAttributeOrNull = type.GetCustomAttributes(typeof(T), false).FirstOrDefault();

            if (toolAttributeOrNull is null)
                return null;

            return toolAttributeOrNull as T;
        }

        private static (PropertyInfo property, T attribute)[] GetPropertiesWithAttribute<T>(Type type) where T : Attribute
        {
            // Get all properties of the type
            var properties = type.GetProperties();

            // Filter properties that have the specified attribute and project them as tuples
            var propertiesWithAttribute = properties
                .Select(p => (property: p, attribute: p.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T))
                .Where(t => t.attribute != null)
                .ToArray();

            return propertiesWithAttribute;
        }
    }
}
