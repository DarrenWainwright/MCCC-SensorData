// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SensorData.Model;

namespace SensorData.Command
{
    public static class SaveLocalWeather
    {
        /// <summary>
        /// Retrieves weather information and stores it. 
        /// </summary>
        /// <param name="eventGridEvent"></param>
        /// <param name="log"></param>
        /// <param name="sensors"></param>
        [FunctionName(nameof(SaveLocalWeather))]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
                                [CosmosDB(databaseName: "MotherCluckers",
                                          collectionName: "WeatherData",
                                          ConnectionStringSetting = "AzureCosmosUrl")] IAsyncCollector<WeatherDocument> weathers)
        {
            Common.Log(log, $"Function: {nameof(SaveSensorEvent)} started. Event Type {eventGridEvent.EventType}");

            string weatherEndpoint = $"{Environment.GetEnvironmentVariable("WeatherApiBase")}?q={Environment.GetEnvironmentVariable("WeatherApiLocation")}&units=metric&appid={Environment.GetEnvironmentVariable("WeatherApiKey")}";

            try
            {
                Common.Log(log, $"Fetch weather data");
                using var client = new HttpClient();
                using var response = await client.GetAsync(weatherEndpoint);
                var responseContent = await response.Content.ReadAsAsync<JObject>();
                if (response.IsSuccessStatusCode)
                {

                    Common.Log(log, $"Have weaher data, preparing to store");

                    var weather = new WeatherDocument(Environment.GetEnvironmentVariable("WeatherApiLocation")
                                                     , responseContent["main"]["temp"].Value<double>()
                                                     , responseContent["main"]["humidity"].Value<double>());

                    weathers.AddAsync(weather);
                }
                else
                {
                    Common.Log(log, $"Failed to fetch weather data. {response.StatusCode}. {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

        }
    }
}
