using EasyOpenAiTools.Library.Tool.Attributes;
using OpenAI.Chat;
using System.Dynamic;
using System.Reflection;

namespace EasyOpenAiTools.Library.Tool
{
    internal class Tool
    {
        public Type Type { get; }
        public ToolAttribute ToolAttribute { get; }
        public List<ToolProperty> ToolProperties { get; }
        public MethodInfo ExecutionMethod { get; }

        public Tool(Type type, ToolAttribute toolAttribute, List<ToolProperty> toolProperties, MethodInfo executionMethod)
        {
            Type = type;
            ToolAttribute = toolAttribute;
            ToolProperties = toolProperties;
            ExecutionMethod = executionMethod;
        }

        public ChatTool ToChatTool()
        {
            var parameterDescription = CreateDynamicObject(ToolProperties);

            return ChatTool.CreateFunctionTool(
                functionName: ToolAttribute.Name,
                functionDescription: ToolAttribute.Description,
                functionParameters: BinaryData.FromObjectAsJson(parameterDescription)
            );
        }

        public async Task<string> Execute(Dictionary<string, string> attributes)
        {
            var instance = Activator.CreateInstance(Type);

            foreach (var toolProperty in ToolProperties)
            {
                var propertyInfo = toolProperty.property;
                var argumentInfo = toolProperty.attribute;

                // Check if property is of type string
                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (attributes.ContainsKey(argumentInfo.Name))
                        // Set the property value to "asdf"
                        propertyInfo.SetValue(instance, attributes[argumentInfo.Name]);
                }
            }

            return await (Task<string>)ExecutionMethod.Invoke(instance, null);
        }

        private static dynamic CreateDynamicObject(List<ToolProperty> toolProperties)
        {
            dynamic obj = new ExpandoObject();
            obj.type = "object";
            obj.properties = new ExpandoObject();
            obj.required = new List<string>();

            foreach (var toolProperty in toolProperties)
            {
                dynamic argumentProperty = new ExpandoObject();
                argumentProperty.type = "string";
                argumentProperty.description = toolProperty.attribute.Description;

                ((IDictionary<string, object>)obj.properties).Add(toolProperty.attribute.Name, argumentProperty);
                obj.required.Add(toolProperty.attribute.Name);
            }

            return obj;
        }
    }
}
