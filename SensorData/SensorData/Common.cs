using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SensorData
{
    public static class Common
    {



        /// <summary>
        /// Return the jtoken value or throw a controlled exception if its missing
        /// </summary>
        /// <param name="data"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object GetOrThrow(JObject data, string v) => data.GetValue(v) ?? throw new ArgumentNullException($"Expected a {v} at data.{v}");

        /// <summary>
        /// Logs a message, formatted with a datetime
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        public static void Log(ILogger log, string message) => log.LogInformation($"{DateTime.UtcNow}\t:\t{message}");
    }
}
