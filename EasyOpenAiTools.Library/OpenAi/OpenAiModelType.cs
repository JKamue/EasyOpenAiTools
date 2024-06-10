namespace EasyOpenAiTools.Library.OpenAi
{
    /// <summary>
    /// The <c>OpenAiModelType</c> class represents different types of OpenAI language models.
    /// </summary>
    public class OpenAiModelType
    {
        /// <summary>
        /// Gets the GPT-4 model type.
        /// </summary>
        public static OpenAiModelType Gpt4 => new OpenAiModelType("gpt-4o");

        /// <summary>
        /// Gets the GPT-3.5 model type (Turbo variant).
        /// </summary>
        public static OpenAiModelType Gpt35 => new OpenAiModelType("gpt-3.5-turbo-0125");

        internal string Name { get; init; }

        private OpenAiModelType(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

        /// <summary>
        /// Implicitly converts an <see cref="OpenAiModelType"/> instance to its string representation.
        /// </summary>
        /// <param name="model">The <see cref="OpenAiModelType"/> instance to convert.</param>
        /// <returns>The name of the model type.</returns>
        public static implicit operator string(OpenAiModelType model) => model.ToString();
    }
}
