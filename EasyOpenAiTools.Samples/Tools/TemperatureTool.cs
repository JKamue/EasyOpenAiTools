using EasyOpenAiTools.Library.Tool.Attributes;
using OpenMeteo;

namespace EasyOpenAiTools.Samples.Tools
{
    [Tool("GetTemperatureAtLocation", 
        "Returns the current temperature of a given city or location." +
        "Use this soley to get the temperature of one place or city." +
        "If only the temperature is needed prefer this function over other functions that give more no needed information.")]
    internal class TemperatureTool
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
}
