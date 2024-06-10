using EasyOpenAiTools.Library.Tool.Attributes;
using OpenMeteo;

namespace EasyOpenAiTools.Samples.Tools
{
    [Tool("GetCurrentTime",
        "Returns the current time as well as timezone information of the user." +
        "Unless specified otherwise give the user his local time and dont use timezone information unless asked.")]
    internal class TimeTool
    {
        [ToolMethod]
        public async Task<string> Execute()
        {
            var weatherClient = new OpenMeteoClient();

            var localNow = DateTime.Now;
            var localTimeAsString = localNow.ToString("R");

            var utcNow = DateTime.UtcNow;
            var utcTimeAsString = utcNow.ToString("R");

            var timeZone = TimeZoneInfo.Local;
            var timeZoneAsString = timeZone.DisplayName;

            return $"local={localTimeAsString} utc={utcTimeAsString} timezone={timeZoneAsString}";
        }
    }
}
