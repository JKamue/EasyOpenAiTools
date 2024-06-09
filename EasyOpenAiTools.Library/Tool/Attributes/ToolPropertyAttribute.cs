namespace EasyOpenAiTools.Library.Tool.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ToolPropertyAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ToolPropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
