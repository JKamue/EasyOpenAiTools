namespace EasyOpenAiTools.Library.Tool.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ToolAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ToolAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
