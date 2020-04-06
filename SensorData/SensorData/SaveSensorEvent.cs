// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.Dynamic;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SensorData
{
    /// <summary>
    /// Stores the data subscribed to, from the sensors published events
    /// </summary>
    public static class SaveSensorEvent
    {
        [FunctionName(nameof(SaveSensorEvent))]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
            [CosmosDB(databaseName: "MotherCluckers",
                      collectionName: "SensorData",
                      ConnectionStringSetting = "AzureCosmosUrl")] IAsyncCollector<dynamic> sensors)
        {
            Common.Log(log, $"Function: {nameof(SaveSensorEvent)} started. Event Type {eventGridEvent.EventType}");

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

            Common.Log(log, $"Sensor document ready for storage : { JsonConvert.SerializeObject(sensor) }");
            sensors.AddAsync(sensor);
            Common.Log(log, "Data Stored. SaveSensorEvent complete");

            //TODO - Figure out why this will not work
            //outdated library?

            //Models.ISensor sensor = eventGridEvent.EventType switch
            //{
            //    "TemperatureChangedEvent" => new Models.TemperatureSensor(sensorId, name,(float)GetOrThrow(data, "temperature_c"), (float)GetOrThrow(data, "temperature_f")),
            //    "HumidityChangedEvent" => new Models.HumiditySensor(sensorId, name, (float)GetOrThrow(data, "humidity"))
            //};
        }


    }
}
