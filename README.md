# EasyOpenAiTools

<h4 align="center">Easily build chatbots with custom functionality on the .NET platform</h4>


<p align="center">
  <a href="#about">About</a> •
  <a href="#goals">Goals</a> •
  <a href="#how-does-it-work">How does it work</a> •
  <a href="#future-developement">Future developement</a>
</p>

## About

EasyOpenAiTools is a lightweight wrapper allowing you to easily build chatbots based on OpenAi.
It is built around the new [openai/openai-dotnet](https://github.com/openai/openai-dotnet) library.


## Goals

Integrating OpenAi LLMs with custom tools and function calling was my main goal and the reason why I created this library.

Registering a tool and running the OpenAi library in a way that makes sure that tools are called correctly takes some boilerplate that would have to be repeated in every project.
I created this library to allow me to easily integrate and reuse the code in different projects in the future. And maybe it will help you too :)

## How does it work

Make sure to check the `EasyOpenAiTools.Samples` project for a running example.

### Creating a Model

To start a cjat you need to supply an OpenAiSettings Object containing the following information:
- Your Api Key
- The Type of Model you want to use (currently Gpt3.5 or Gpt4)
- The initial prompt telling the model how to behave [check here for inspiration](https://platform.openai.com/docs/guides/prompt-engineering/tactics)

```c#
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var openAiModelType = OpenAiModelType.Gpt4;
var initialPrompt = "Answer all questions by users in a brief and concise way.";

var openAiSettings = new OpenAiSettings(openAiApiKey, openAiModelType, initialPrompt);
var chat = OpenAiChatFactory.StartNewChat(openAiSettings, logger);
```

After creating the chat using it is as easy as:

```c#
var response = chat.Ask("Why is the moon flying away further from earth?").Result;
Console.WriteLine($"[Assistant]: {response}");

// [Assistant]: The Moon is gradually moving away from the Earth due to tidal forces. 
// The Earth's rotation causes tidal bulges in the oceans, which transfer angular momentum 
// to the Moon, causing it to slowly accelerate in its orbit. As the Moon gains angular momentum, 
// its orbit becomes slightly larger, leading to an increase in the distance between the Earth and 
// the Moon. This process occurs at a rate of about 3.8 centimeters per year.

var response = chat.Ask("When will the moon be to small to fully cover the sun?").Result;

// [Assistant]: The phenomenon where the Moon covers the Sun completely during a solar eclipse is known 
// as a total solar eclipse. Due to the Moon's gradual increase in distance from the Earth, its apparent 
// size in the sky is shrinking. Scientists estimate that in about 600 million years, the Moon will no
// longer be able to fully cover the Sun, and total solar eclipses will no longer occur. Instead, we will
// only experience annular eclipses, where the Moon appears smaller than the Sun and leaves a ring of the
// Sun visible around its edges.
```


### Creating Tools

Chatting with the LLM is nice and all, but the real fun lies in providing the LLM with custom Tools.
These tools can integrate with APIs or Databases, anything you can imagine.

The following class is automatically detected by EasyOpenAiTools and added to the list of tools available to the LLM.
For this auto discovery to work you need to provide 3 types of attributes:

- `Tool`: The tool Attribute is set at the class level, marking it as a tool. 
You can use it to set the name and the description of the tool. 
This description will be used by the LLM to identify and use the tool so make sure to explain it correctly and experiment.
- `ToolProperty`: These are Arguments you want the LLM to set when calling your custom Tool. Again each Argument has a name and Description.
- `ToolMethod`: This is the method that will be called when the LLM calls your tool

```c#
[Tool("GetWeatherAtLocation", 
      "Returns the current temperature of a given city or location")]
internal class WeatherToolNew
{
    [ToolProperty("Location", "The name of a city. This only takes the city name, not numbers or other inputs.")]
    public string Location { get; init; }

    [ToolMethod]
    public async Task<string> Execute()
    {
        var weatherClient = new OpenMeteoClient();
        var weatherData = await weatherClient.QueryAsync(Location);

        return
            $"Weather {Location} now: " +
            weatherData.Current.Temperature +
            weatherData.CurrentUnits.Temperature;
    }
}
```

No further action is required. When combining this with the model creation from the `Creating a Model` section you can easily start using the tool.

```
[User]: What is the current Temperature in Berlin?
[Assistant]: The current temperature in Berlin is 16.9°C.
[User]: How did you find this out?
[Assistant]: I used a weather API to retrieve the current temperature in Berlin.
[User]: Which tool did you use for that?
[Assistant]: I used the `GetWeatherAtLocation` function to find out the current temperature in Berlin.
```

Multiple calls in parallel are also possible:
```
[User]: Where is it currently 5°C?
[Assistant]: Currently, it is not 5°C in Reykjavik, Oslo, Moscow, Helsinki, Stockholm, or Vancouver.
[User]: What are their respective temperatures and why did you choose these places?
[Assistant]: Here are the current temperatures for the cities:

- Reykjavik: 8.6°C
- Oslo: 11.9°C
- Moscow: 16.8°C
- Helsinki: 12.2°C
- Stockholm: 11.3°C
- Vancouver: 21.7°C

As for why I chose these locations, they are known for having relatively cool climates, especially during 
certain times of the year, which made them good candidates when looking for a place where it might be 5°C.
```

A few things to note:

- The last example only worked with GPT 4. GPT 3.5 always thought it could use the `5°C` as a place name and supplied it straight to the API. Overall GPT 4 gives a lot better results when using tools but costs noticeably more as well

- A big limitation though is the context size of the LLM.
You should not flat out return a complete dataset through a tool.
Instead a tool should provide small functions that enhance the experience.

- Also note the [128 Tool limit](https://platform.openai.com/docs/assistants/how-it-works/creating-assistants) 

## Future developement

I plan to rework a bunch of things. This includes a way to predict the costs generated by a chat as well as implementing strategies that cut off the context at a certain length or summarize it to save tokens.
I also want to add support for enums as input in the future as well as any other input types the OpenAi supports. I also plan to add optional properties.

Integrating image support or any other [openai/openai-dotnet](https://github.com/openai/openai-dotnet) functions is not planned at the moment but might be added to the todo if I need them in any project.

Since this project is open source you are very welcome to contribute and implement any improvements or features you can imagine.

## Apendix

### Catch error messages with code
By default issues or errors like an issue due to the ContentFilter will be communicated as text. 
Running `await chat.Ask(userMessage)` will return `Response failed because '{reason}'`.
Then the user can adapt their request.

If you want to instead catch those errors and communicate them to users differently you can use the following example:

```c#
var responseResult = await chat.AskWithResult(userMessage);

if (responseResult.IsFailure)
{
    // Handle
}
else
{
    var response = responseResult.Value;
    Console.WriteLine($"[Assistant]: {response}");
}
```

Results use [vkhorikov/CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) under the hood.

### Multiple Functions with Debugging

This is a small example highlighting how the model can intelligently use functions and also showing how to debug easily.

First you need to create a custom logger:

```c#
using var factory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug);
    });
var logger = factory.CreateLogger("EasyOpenAi");
```

You can then supply this logger when creating a new Chat:

```c#
var chat = OpenAiChatFactory.StartNewChat(openAiSettings, logger);
```

The resulting output will look like this
```
dbug: EasyOpenAi[0]
      2 tools found, will try to add them
dbug: EasyOpenAi[0]
      EasyOpenAiTools.Samples.Tools.WeatherForcecastTool successfully add as tool
dbug: EasyOpenAi[0]
      EasyOpenAiTools.Samples.Tools.TemperatureTool successfully add as tool
LLM chat (write exit to close):
[User]: What is the current temperature in karlsruhe?
dbug: EasyOpenAi[0]
      Executing GetTemperatureAtLocation with arguments '{"Location":"Karlsruhe"}'
[Assistant]: The current temperature in Karlsruhe is 18.6°C.
[User]: Can you give me more details on the weather at the moment?
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location":"Karlsruhe","Date":"0"}'
[Assistant]: Here are the detailed weather conditions for Karlsruhe at the moment:
- **Maximum Temperature:** 22.1°C
- **Minimum Temperature:** 13°C
- **Apparent Temperature Range:** 11.6°C to 19.1°C
- **Precipitation:** 0.7 mm
- **Rain:** 0.7 mm
- **No Showers or Snowfall**
- **Precipitation Hours:** 2 hours
- **Max Windspeed:** 51.5 km/h
- **Wind Direction:** 265°
[User]: When will the weather be the nices in karlsruhe this week?
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "1"}'
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "2"}'
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "3"}'
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "4"}'
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "5"}'
dbug: EasyOpenAi[0]
      Executing GetWeatherForecastForLocation with arguments '{"Location": "Karlsruhe", "Date": "6"}'
[Assistant]: Based on the weather forecast, the nicest day in Karlsruhe this week is likely to be:

- **June 13**:
  - **Maximum Temperature:** 21.7°C
  - **Minimum Temperature:** 8.8°C
  - **Apparent Temperature Range:** 7.2°C to 20.9°C
  - **Precipitation:** 0 mm
  - **Max Windspeed:** 18.4 km/h
  - **Wind Direction:** 159°

This day has clear conditions with no precipitation, pleasant temperatures, and moderate wind speed.
```