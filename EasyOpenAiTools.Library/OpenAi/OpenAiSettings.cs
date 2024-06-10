namespace EasyOpenAiTools.Library.OpenAi;

/// <summary>
/// The <c>OpenAiSettings</c> record represents the settings required to configure an OpenAI chat.
/// </summary>
public record OpenAiSettings(
  /// <summary>
  /// The OpenAI API key used to authenticate requests.
  /// </summary>
  string OpenAiApiKey,

  /// <summary>
  /// The type of OpenAI language model to use for the chat session.
  /// </summary>
  OpenAiModelType OpenAiModel,

  /// <summary>
  /// The initial prompt to start the chat session with.
  /// </summary>
  string InitialPrompt
);
