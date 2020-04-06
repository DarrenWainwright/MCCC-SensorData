// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using System.Dynamic;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCCC.Sensors.Data
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
            Logger.Log(log, $"Function: {nameof(SaveSensorEvent)} started. Event Type {eventGridEvent.EventType}");

            var data = (JObject)eventGridEvent.Data;

            dynamic sensor = new ExpandoObject();
            sensor.id = eventGridEvent.Id;
            sensor.sensorId = GetOrThrow(data, "sensor_id");
            sensor.name = GetOrThrow(data, "name");

            switch (eventGridEvent.EventType)
            {
                case "TemperatureChangedEvent":
                    sensor.celcius = GetOrThrow(data, "temperature_c");
                    sensor.fahrenheit = GetOrThrow(data, "temperature_f");
                    break;
                case "HumidityChangedEvent":
                    sensor.humidity = GetOrThrow(data, "humidity");
                    break;
            }

            Logger.Log(log, $"Sensor document ready for storage : { JsonConvert.SerializeObject(sensor) }");
            sensors.AddAsync(sensor);
            Logger.Log(log, "Data Stored. SaveSensorEvent complete");

            //TODO - Figure out why this will not work
            //outdated library?

            //Models.ISensor sensor = eventGridEvent.EventType switch
            //{
            //    "TemperatureChangedEvent" => new Models.TemperatureSensor(sensorId, name,(float)GetOrThrow(data, "temperature_c"), (float)GetOrThrow(data, "temperature_f")),
            //    "HumidityChangedEvent" => new Models.HumiditySensor(sensorId, name, (float)GetOrThrow(data, "humidity"))
            //};
        }

        private static object GetOrThrow(JObject data, string v) => data.GetValue(v) ?? throw new ArgumentNullException($"Expected a {v} at data.{v}");

        private static class Logger
        {
            public static void Log(ILogger log, string message) => log.LogInformation($"{DateTime.UtcNow}\t:\t{message}");

        }
    }
}
