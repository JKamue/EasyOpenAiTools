namespace EasyOpenAiTools.Library.Tool.Attributes
{
    /// <summary>
    /// The <c>ToolPropertyAttribute</c> class is a custom attribute used to mark a property as an argument of a Tool.
    /// This allows the LLM to understand which arguments it needs to ask for from the user and also tells it how
    /// to use the property, including any necessary formats or ranges.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to properties multiple times and is intended for properties with either an <c>init</c> or <c>set</c> accessor.
    /// Only properties of the type <c>string</c> are supported.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ToolPropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the property. The description should include any necessary formats, ranges, or other constraints.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolPropertyAttribute"/> class with the specified name and description.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="description">The description of the property, including guides on usage as well as necessary formats and ranges.</param>
        public ToolPropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
