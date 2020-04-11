// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Dynamic;
using System.Net.Http;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
        public static async void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
                                [CosmosDB(databaseName: "MotherCluckers",
                                          collectionName: "WeatherData",
                                          ConnectionStringSetting = "AzureCosmosUrl")] IAsyncCollector<dynamic> weathers)
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

                    var eventData = (JObject)eventGridEvent.Data;

                    dynamic weather = new ExpandoObject();
                    weather.id = Guid.NewGuid().ToString();
                    weather.location = Environment.GetEnvironmentVariable("WeatherApiLocation");
                    weather.triggerEvent = eventGridEvent.EventType;
                    weather.triggerSensor = Common.GetOrThrow(eventData, "sensor_id");
                    weather.celcius = responseContent["main"]["temp"].Value<double>();
                    weather.fahrenheit = ((9.0 / 5.0) * weather.celcius) + 32;
                    weather.humidity = responseContent["main"]["humidity"].Value<double>();
                    weather.dateCreated = eventGridEvent.EventTime;
                    weathers.AddAsync(weather);
                    Common.Log(log, $"Weather document stored");
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
