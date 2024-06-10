using EasyOpenAiTools.Library.Tool.Attributes;
using OpenMeteo;

namespace EasyOpenAiTools.Samples.Tools
{
    [Tool("GetWeatherForecastForLocation",
        "Returns the current detailed weather information as well as a forecast for a given location." +
        "The Location only takes the name of cities and other places as an input.")]
    internal class WeatherForcecastTool
    {
        [ToolProperty("Location", "The name of a city. This only takes the city name, not numbers or other inputs.")]
        public string Location { get; init; }

        [ToolProperty("Date", "The relative date. Use 0 for today, 1 for tomorrow and so on. The maximum is 6. It must be an int between 0 and 6")]
        public string Date { get; init; }

        [ToolMethod]
        public async Task<string> Execute()
        {
            var parseSuccess = int.TryParse(Date, out int relativeDay);
            if (!parseSuccess || relativeDay < 0 || relativeDay > 6)
                return "The Date must be supplied as an a number between 0 and 6";

            var weatherClient = new OpenMeteoClient();
 
            WeatherForecastOptions options = new WeatherForecastOptions
            {
                Current = CurrentOptions.All,
                Daily = DailyOptions.All
            };

            var weatherData = await weatherClient.QueryAsync(Location, options);


            var relevantData = new
            {
                units = new {
                    temperature = weatherData.CurrentUnits.Temperature,
                    precipitation = weatherData.CurrentUnits.Precipitation,
                    rain = weatherData.CurrentUnits.Rain,
                    showers = weatherData.CurrentUnits.Showers,
                    snowfall = weatherData.CurrentUnits.Snowfall,
                    windspeed = weatherData.CurrentUnits.Windspeed_10m,
                    winddirection = weatherData.CurrentUnits.Winddirection_10m
                },
                time = weatherData.Daily.Time[relativeDay],
                temp_2m_max = weatherData.Daily.Temperature_2m_max[relativeDay],
                temp_2m_min = weatherData.Daily.Temperature_2m_min[relativeDay],
                
                apparent_temp_may = weatherData.Daily.Apparent_temperature_max[relativeDay],
                apparent_temp_min = weatherData.Daily.Apparent_temperature_min[relativeDay],
                precipitation_sum = weatherData.Daily.Precipitation_sum[relativeDay],
                rain_sum = weatherData.Daily.Rain_sum[relativeDay],
                showers_sum = weatherData.Daily.Showers_sum[relativeDay],
                precipitation_hours = weatherData.Daily.Precipitation_hours[relativeDay],
                windspeed_10m_max = weatherData.Daily.Windgusts_10m_max[relativeDay],
                winddirection = weatherData.Daily.Winddirection_10m_dominant[relativeDay],
                snowfall_sum = weatherData.Daily.Snowfall_sum[relativeDay]
            };

            var weatherDataAsString = BinaryData.FromObjectAsJson(relevantData);

            return weatherDataAsString.ToString();
        }
    }
}
