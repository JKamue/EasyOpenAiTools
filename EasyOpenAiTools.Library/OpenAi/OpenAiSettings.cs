namespace EasyOpenAiTools.Library.OpenAi;
public record OpenAiSettings(
  string OpenAiApiKey,
  OpenAiModelType OpenAiModel,
  string InitialPrompt
);
