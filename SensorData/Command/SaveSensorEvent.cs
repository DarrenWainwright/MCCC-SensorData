// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SensorData.Model;

namespace SensorData.Command
{
    /// <summary>
    /// Stores the data subscribed to, from the sensors published events
    /// </summary>
    public static class SaveSensorEvent
    {
        [FunctionName(nameof(SaveSensorEvent))]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
            [CosmosDB(databaseName: "MotherCluckers",
                      collectionName: "SensorData",
                      ConnectionStringSetting = "AzureCosmosUrl")] IAsyncCollector<IDHT22> sensors)
        {
            Common.Log(log, $"Function: {nameof(SaveSensorEvent)} started. Event Type {eventGridEvent.EventType}");

            try
            {
                var data = (JObject)eventGridEvent.Data;

                IDHT22 sensor = eventGridEvent.EventType switch
                {
                    "TemperatureChangedEvent" => new TemperatureSensorEvent(data["sensor_id"].Value<string>(), data["name"].Value<string>(), data["temperature_c"].Value<double>()),
                    "HumidityChangedEvent" => new HumiditySensorEvent(data["sensor_id"].Value<string>(), data["name"].Value<string>(), data["humidity"].Value<double>()),
                    _ => throw new InvalidOperationException($"Event type {eventGridEvent.EventType} unsupported")
                };

                if (sensor != null)
                    await sensors.AddAsync(sensor);


            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }

        }
    }
}
