// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using System.Dynamic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        public async void Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            Common.Log(_logger, $"Function: {nameof(SaveSensorHeartbeat)} started. Event Type {eventGridEvent.EventType}");

            Common.Log(_logger, "Fetch any existing sensor.");
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(eventGridEvent.Data.ToString());

            try
            {

                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM s where Lower(s.name) = @name AND Lower(s.type) = @type")
                                                                    .WithParameter("@name", data.name.ToLower())
                                                                    .WithParameter("@type", data.type.ToLower());
                var query = _container.GetItemQueryIterator<ExpandoObject>(queryDefinition);
                dynamic sensor = (await query.ReadNextAsync()).FirstOrDefault();

                if (sensor == null)
                {
                    Common.Log(_logger, "No sensor found. Create one.");
                    // create one
                    sensor = new ExpandoObject();
                    sensor.id = Guid.NewGuid();
                    sensor.name = data.name;
                    sensor.type = data.type;
                    sensor.heartbeatInterval = data.heartbeat_interval;
                    sensor.lastConnected = DateTime.UtcNow;
                }
                else
                {
                    Common.Log(_logger, "Sensor found. Update last connected");
                    //update one.
                    sensor.heartbeatInterval = data.heartbeat_interval;
                    sensor.lastConnected = DateTime.UtcNow;
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
