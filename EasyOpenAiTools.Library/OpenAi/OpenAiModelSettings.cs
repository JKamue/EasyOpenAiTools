namespace EasyOpenAiTools.Library.OpenAi;
public record OpenAiModelSettings(
  string OpenAiApiKey,
  OpenAiModelType OpenAiModel,
  string InitialPrompt
);
