namespace EasyOpenAiTools.Library.Tool.Attributes
{
    /// <summary>
    /// The <c>ToolMethodAttribute</c> class is a custom attribute used to mark the method of a tool.
    /// Each tool must mark one and only one method with the <c>ToolMethodAttribute</c> attribute.
    /// This attribute is used internally to identify which methods are executable by the LLM when a certain tool is called.
    /// The name of the method to which this attribute is applied is not communicated to the LLM and can be chosen freely by the developer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ToolMethodAttribute : Attribute
    {

    }
}
