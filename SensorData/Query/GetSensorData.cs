using System.Collections.Generic;
using System.Linq;
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
    public class GetSensorData
    {

        private readonly ILogger<GetSensorData> _logger;
        private readonly Container _container;

        public GetSensorData(CosmosClient cosmosClient, ILogger<GetSensorData> logger)
        {
            _container = cosmosClient.GetDatabase("MotherCluckers").GetContainer("SensorData");
            _logger = logger;
        }



        [FunctionName(nameof(GetSensorData))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sensors/{name}/data")] HttpRequest req,
            string name)
        {

            try
            {

                Common.Log(_logger, $"_{nameof(GetSensorData)} started with route {name}");

                name = name?.ToLower();
                var query = _container.GetItemLinqQueryable<IDictionary<string, object>>()
                            .Where(s => s["name"].ToString().ToLower() == name);

                if (int.TryParse(req.Query["skip"], out int skip))
                    query.Skip(skip);

                if (int.TryParse(req.Query["take"], out int take))
                    query.Skip(take);

                query.OrderByDescending(s => s["timestamp"]);

                var iterator = query.ToFeedIterator();

                List<object> results = new List<object>();
                //Asynchronous query execution
                while (iterator.HasMoreResults)
                    foreach (var item in await iterator.ReadNextAsync())
                    {

                        item.Remove("_rid");
                        item.Remove("_self");
                        item.Remove("_etag");
                        item.Remove("_attachments");

                        if (item.ContainsKey("celcius"))
                            item.Add("datatype", "Temperature");
                        if (item.ContainsKey("humidity"))
                            item.Add("datatype", "Humidity");

                        results.Add(item);

                    }

                return new OkObjectResult(results);

            }
            catch (System.Exception ex)
            {

                return new BadRequestObjectResult(ex);
            }
        }
    }
}

