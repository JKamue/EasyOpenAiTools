using EasyOpenAiTools.Library.Tool.Attributes;
using System.Reflection;

namespace EasyOpenAiTools.Library.Tool
{
    internal record ToolProperty(
        PropertyInfo property,
        ToolPropertyAttribute attribute
    );
}
