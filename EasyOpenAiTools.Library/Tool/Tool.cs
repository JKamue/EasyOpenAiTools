using CSharpFunctionalExtensions;
using EasyOpenAiTools.Library.Helper;
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

        public static Result<Tool> LoadFrom(Type type)
        {
            // Ensure type has tool attribute
            Result<ToolAttribute> toolAttributeResult = AttributeHelper
                .ReadAttributeOfClass<ToolAttribute>(type)
                .ToResult($"Type {type.Namespace}{type.Name} does not have the {nameof(Tool)} attribute to indicate it as a Tool");

            // Ensure type has one tool method
            Result<MethodInfo> executionMethodResult = GetExecutionMethod(type);

            Result methodAndToolResult = Result.Combine(toolAttributeResult, executionMethodResult);

            if (methodAndToolResult.IsFailure)
                return Result.Failure<Tool>(methodAndToolResult.Error);

            // Make List of all Properties
            var propertyList = GetToolPropertyList(type);

            // Ensure that all these properties are type string
            var propertiesThatAreNotStrings = propertyList.Where(p => p.property.PropertyType != typeof(string)).ToList();
            if (propertiesThatAreNotStrings.Any())
                return Result.Failure<Tool>($"Type {type.Namespace}{type.Name} has properties marked with {nameof(ToolPropertyAttribute)} that are not strings");

            return new Tool(type, toolAttributeResult.Value, propertyList, executionMethodResult.Value);
        }

        private static Result<MethodInfo> GetExecutionMethod(Type type)
        {
            var allMethodsWithToolAttribute = AttributeHelper.GetMethodsWithAttribute<ToolMethodAttribute>(type);
            if (allMethodsWithToolAttribute.Count() == 0)
                return Result.Failure<MethodInfo>($"Type {type.Namespace}{type.Name} does not have a method with {nameof(ToolMethodAttribute)} attribute");

            if (allMethodsWithToolAttribute.Count() > 1)
                return Result.Failure<MethodInfo>($"Type {type.Namespace}{type.Name} has to many methods with {nameof(ToolMethodAttribute)} attribute");

            return allMethodsWithToolAttribute.First();
        }

        private static List<ToolProperty> GetToolPropertyList(Type type)
        {
            var propertiesWithToolAttribute = AttributeHelper.GetPropertiesWithAttribute<ToolPropertyAttribute>(type);
           return propertiesWithToolAttribute
                            .Select(propertyWithToolAttribute =>
                                new ToolProperty(
                                    propertyWithToolAttribute.property,
                                    propertyWithToolAttribute.attribute))
                            .ToList();
        }

        private Tool(Type type, ToolAttribute toolAttribute, List<ToolProperty> toolProperties, MethodInfo executionMethod)
        {
            Type = type;
            ToolAttribute = toolAttribute;
            ToolProperties = toolProperties;
            ExecutionMethod = executionMethod;
        }

        public ChatTool ToChatTool()
        {
            var parameterDescription = CreateDynamicObject();
            var parameterDescriptionAsJson = BinaryData.FromObjectAsJson(parameterDescription);

            return ChatTool.CreateFunctionTool(
                functionName: ToolAttribute.Name,
                functionDescription: ToolAttribute.Description,
                functionParameters: parameterDescriptionAsJson
            );
        }

        public async Task<string> Execute(Dictionary<string, string> attributes)
        {
            var instance = Activator.CreateInstance(Type);

            foreach (var toolProperty in ToolProperties)
            {
                var propertyInfo = toolProperty.property;
                var argumentInfo = toolProperty.attribute;

                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (attributes.ContainsKey(argumentInfo.Name))
                        propertyInfo.SetValue(instance, attributes[argumentInfo.Name]);
                }
            }

            return await (Task<string>)ExecutionMethod.Invoke(instance, null);
        }

        private dynamic CreateDynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.type = "object";
            obj.properties = new ExpandoObject();
            obj.required = new List<string>();

            foreach (var toolProperty in ToolProperties)
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
