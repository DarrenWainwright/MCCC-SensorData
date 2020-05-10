using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SensorData.Query
{
    public class GetSensors
    {
        private readonly ILogger<GetSensors> _logger;
        private readonly Container _container;

        public GetSensors(CosmosClient cosmosClient, ILogger<GetSensors> logger)
        {
            _container = cosmosClient.GetDatabase("MotherCluckers").GetContainer("Sensor");
            _logger = logger;
        }


        [FunctionName(nameof(GetSensors))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sensors")] HttpRequest req)
        {
            Common.Log(_logger, $"Function: {nameof(GetSensors)} started with a HTTP Trigger");

            try
            {
                // At this time, just return all sensors data
                var iterator = _container.GetItemLinqQueryable<dynamic>().ToFeedIterator();

                List<Model.Sensor> results = new List<Model.Sensor>();
                //Asynchronous query execution
                while (iterator.HasMoreResults)
                    foreach (var item in await iterator.ReadNextAsync())
                        results.Add(new Model.Sensor((string)item.id, (string)item.name, (string)item.type, (int)item.heartbeatInterval, (DateTime)item.lastConnected));

                return new OkObjectResult(results);
            }
            catch (System.Exception ex)
            {

                return new BadRequestObjectResult(ex.Message);
            }


        }
    }
}
