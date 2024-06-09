using EasyOpenAiTools.Library.Tool.Attributes;
using OpenMeteo;

namespace EasyOpenAiTools.Samples.Tools
{
    [Tool("GetWeatherAtLocation", 
        "Returns the current temperature of a given city or location. This does not convert temperatures or does any operations other than looking up the temperature of a set place. Use this soley to get the temperature of one place")]
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
}
