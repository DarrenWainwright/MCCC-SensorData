// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SensorData.Models;

namespace SensorData.Command
{
    /// <summary>
    /// Stores the data subscribed to, from the sensors published events
    /// </summary>
    public static class SaveSensorEvent
    {
        [FunctionName(nameof(SaveSensorEvent))]
        public static async Task<IActionResult> Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
            [CosmosDB(databaseName: "MotherCluckers",
                      collectionName: "SensorData",
                      ConnectionStringSetting = "AzureCosmosUrl")] IAsyncCollector<dynamic> sensors)
        {
            Common.Log(log, $"Function: {nameof(SaveSensorEvent)} started. Event Type {eventGridEvent.EventType}");

            IActionResult result;
            try
            {
                var data = (JObject)eventGridEvent.Data;
                dynamic sensor = new ExpandoObject();
                sensor.id = eventGridEvent.Id;
                sensor.sensorId = Common.GetOrThrow(data, "sensor_id");
                sensor.name = Common.GetOrThrow(data, "name");

                switch (eventGridEvent.EventType)
                {
                    case "TemperatureChangedEvent":
                        sensor.celcius = Common.GetOrThrow(data, "temperature_c");
                        sensor.fahrenheit = Common.GetOrThrow(data, "temperature_f");
                        break;
                    case "HumidityChangedEvent":
                        sensor.humidity = Common.GetOrThrow(data, "humidity");
                        break;
                }
                sensors.AddAsync(sensor);

                result = new OkObjectResult(sensor);

            }
            catch (System.Exception ex)
            {
                result = new BadRequestObjectResult(new Error(ex));
            }

            return result;

        }
    }
}
