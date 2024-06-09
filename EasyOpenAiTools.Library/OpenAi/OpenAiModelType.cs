namespace EasyOpenAiTools.Library.OpenAi
{
    public class OpenAiModelType
    {
        public static OpenAiModelType Gpt4 => new OpenAiModelType("gpt-4o");
        public static OpenAiModelType Gpt35 => new OpenAiModelType("gpt-3.5-turbo-0125");

        public string Name { get; init; }

        private OpenAiModelType(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        public static implicit operator string(OpenAiModelType model) => model.ToString();
    }
}
