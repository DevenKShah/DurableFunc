using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Azure.Storage.Queues;

namespace DurableFunc.HttpTrigger
{
    public static class KickOffDurableOrchestrationFunction
    {
        [FunctionName("KickOffDurableOrchestrationFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string numberOfMessages = req.Query["numberofmessages"];
            if(string.IsNullOrWhiteSpace(numberOfMessages)) return new BadRequestResult();
            if(!int.TryParse(numberOfMessages, out var noOfMsgs)) return new BadRequestResult();

            await SendMessages(noOfMsgs);

            string responseMessage = $"{noOfMsgs} were triggered successfully.";

            return new OkObjectResult(responseMessage);
        }

        private static async Task SendMessages(int noOfMsgs)
        {
            // Get the connection string from app settings
            string connectionString = "UseDevelopmentStorage=true";

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            var queueClient = new QueueClient(connectionString, "filereceived-queue");

            for (var i = 0; i < noOfMsgs; i++)
            {
                var filename = Guid.NewGuid().ToString();
                var msg1 = System.Text.Json.JsonSerializer.Serialize(new { FileName = filename, FileExtension = "TN"});
                await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(msg1)));
                //var msg2 = JsonSerializer.Serialize(new FileReceivedMessage(filename, "TS"));	
                //queueClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(msg2)));
            }
        }
    }
}
