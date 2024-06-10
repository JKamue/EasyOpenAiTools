namespace EasyOpenAiTools.Library.Tool.Attributes
{
    /// <summary>
    /// The <c>ToolAttribute</c> class is a custom attribute used to mark a class as a tool. 
    /// This facilitates the automatic discovery and registration of tools, allowing them to be loaded 
    /// automatically when creating an <c>OpenAiChat</c>.
    /// </summary>
    /// <remarks>
    /// For a class to be a valid tool it needs exactly one function of any name to be marked with the 
    /// <see cref="ToolMethodAttribute"/>. If your tool needs parameters add public properties and mark 
    /// them with the <see cref="ToolPropertyAttribute"/>. A tool does not need any properties. An example
    /// for a tool with no properties would be a tool returning the current time.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ToolAttribute : Attribute
    {
        /// <summary>
        /// Returns the name of the tool. This is used by the LLM to choose tools and is also included
        /// in logs if logging is activated. It is recommended to choose a representative, recognizable name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the description of the tool. The LLM uses this description when choosing a tool.
        /// Ensure that the description reflects the uses of the tool and maybe also mentions what
        /// the tool cannot be used for.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolAttribute"/> class with the specified name and description.
        /// The following information is used by the LLM when selecting a tool. Use them to outline what your tool is
        /// capable of and, if needed, also mention what the tool cannot be used for.
        /// </summary>
        /// <param name="name">The name of the tool. Used by the LLM to decide which tool to use.</param>
        /// <param name="description">The description of the tool. Used by the LLM to decide which tool to use.</param>
        public ToolAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
