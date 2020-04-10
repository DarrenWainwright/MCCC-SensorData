// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
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
    public class SaveSensorHeartbeat
    {
        private readonly ILogger<SaveSensorHeartbeat> _logger;
        private readonly Container _container;

        public SaveSensorHeartbeat(ILogger<SaveSensorHeartbeat> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            var db = cosmosClient.GetDatabase("MotherCluckers");
            _container = db.GetContainer("Sensor");
        }

        [FunctionName(nameof(SaveSensorHeartbeat))]
        public async Task<IActionResult> Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            Common.Log(_logger, $"Function: {nameof(SaveSensorHeartbeat)} started. Event Type {eventGridEvent.EventType}");

            IActionResult result = null;
            var data = (JObject)eventGridEvent.Data;
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM s where Lower(s.name) = @name")
                                                                     .WithParameter("@name", Common.GetOrThrow(data, "name"));
                dynamic r = _container.GetItemQueryIterator<ExpandoObject>(queryDefinition);

                //if (r == null)



                //    r.Name = "Dd";
                //List<T> results = new List<T>();
                //while (r.HasMoreResults)
                //    results.AddRange(await r.ReadNextAsync());

                //return results;

                //var sensor = _container.GetItemQueryIterator()


                //var data = (JObject)eventGridEvent.Data;
                //dynamic sensor = new ExpandoObject();
                //sensor.id = eventGridEvent.Id;
                //sensor.type = Common.GetOrThrow(data, "type");
                //sensor.name = Common.GetOrThrow(data, "name");
                //sensor.heartbeat = Common.GetOrThrow(data, "heartbeat_interval");
                ////sensors.AddAsync(sensor);
                //Common.Log(log, $"Sensor data stored.");

                //result = new OkObjectResult(sensor);

            }
            catch (System.Exception ex)
            {
                result = new BadRequestObjectResult(new Error(ex));
            }

            return result;

        }
    }
}
