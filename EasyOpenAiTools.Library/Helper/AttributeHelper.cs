using CSharpFunctionalExtensions;
using System.Reflection;

namespace EasyOpenAiTools.Library.Helper
{
    internal class AttributeHelper
    {
        public static Maybe<T> ReadAttributeOfClass<T>(Type type) where T : Attribute
        {
            var attributeList = type.GetCustomAttributes(typeof(T), false);

            if (attributeList.FirstOrDefault() is not T attribute)
                return Maybe.None;

            return attribute;
        }

        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(Type type) where T : Attribute
        {
            return type
                .GetMethods()
                .Where(m => m.GetCustomAttribute<T>() != null);
        }

        public static (PropertyInfo property, T attribute)[] GetPropertiesWithAttribute<T>(Type type) where T : Attribute
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
