// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SensorData.Model;

namespace SensorData.Command
{


    /// <summary>
    /// Listens to the heartbeat from the sensor. 
    /// Uses this data to create the sensor data object in the database
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
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            Common.Log(_logger, $"Function: {nameof(SaveSensorHeartbeat)} started. Event Type {eventGridEvent.EventType}");

            Common.Log(_logger, "Fetch any existing sensor.");
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(eventGridEvent.Data.ToString());

            try
            {

                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM s where Lower(s.name) = @name AND Lower(s.type) = @type")
                                                                    .WithParameter("@name", data.name.ToLower())
                                                                    .WithParameter("@type", data.type.ToLower());
                var query = _container.GetItemQueryIterator<Heartbeat>(queryDefinition);
                var sensor = (await query.ReadNextAsync()).FirstOrDefault();

                if (sensor == null)
                    sensor = new Heartbeat(data.name, data.type, (int)data.heartbeat_interval);
                else
                {
                    sensor.HeartbeatInterval = (int)data.heartbeat_interval;
                    sensor.LastConnected = DateTime.UtcNow;
                }

                await _container.UpsertItemAsync(sensor);

            }
            catch (System.Exception ex)
            {

                throw new InvalidOperationException(ex.Message, ex);
            }

        }
    }
}
