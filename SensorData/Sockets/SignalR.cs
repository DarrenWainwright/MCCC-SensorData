//// Default URL for triggering event grid function in the local environment.
//// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
//using System.Dynamic;
//using Microsoft.Azure.EventGrid.Models;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.EventGrid;
//using Microsoft.Azure.WebJobs.Extensions.SignalRService;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

//namespace SensorData.Sockets
//{
//    public static class SignalR
//    {

//        const string HUB_NAME = "mccc";

//        [FunctionName("negotiate")]
//        public static SignalRConnectionInfo GetSignalRInfo(
//            [EventGridTrigger]EventGridEvent eventGridEvent,
//            [SignalRConnectionInfo(HubName = HUB_NAME)] SignalRConnectionInfo connectionInfo)
//        {
//            return connectionInfo;
//        }


//        [FunctionName("Heartbeat")]
//        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log,
//            [SignalR(HubName = HUB_NAME)] IAsyncCollector<SignalRMessage> signalRMessages)
//        {


//            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(eventGridEvent.Data.ToString());
//            Model.Heartbeat hb = new Model.Heartbeat(data.name, data.type, (int)data.heartbeat_interval);


//            signalRMessages.AddAsync(
//                new SignalRMessage
//                {
//                    Target = "heartbeatReceived",
//                    Arguments = new[] { hb }
//                });

//        }
//    }
//}
