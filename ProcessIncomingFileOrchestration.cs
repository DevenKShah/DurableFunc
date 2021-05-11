using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunc.Function
{
    public class DurableFunc
    {
        private readonly DurableFuncDbContext dbContext;

        public DurableFunc(DurableFuncDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [FunctionName(nameof(NewFile))] 
        public async Task NewFile(
            [QueueTrigger("filereceived-queue", Connection = "AzureWebJobsStorage")] string message,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var file = JsonSerializer.Deserialize<FileReceivedMessage>(message);
            log.LogWarning($"Incoming file message received for {file}");
            await starter.StartNewAsync<FileReceivedMessage>(nameof(IncomingMessageWorkflow), file);
        }

        [FunctionName(nameof(IncomingMessageWorkflow))]
        public async Task IncomingMessageWorkflow([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var file = context.GetInput<FileReceivedMessage>();
            log.LogInformation($"Incoming message workflow triggered for file '{file}'");
            var lockId = new EntityId("File", Guid.NewGuid().ToString());
            using (await context.LockAsync(lockId))
            {
                await context.CallActivityAsync<string>(nameof(SaveFile), file);
            }
        }

        [FunctionName(nameof(SaveFile))]
        public async Task SaveFile(
            //[ActivityTrigger] IDurableActivityContext ctx,
            [ActivityTrigger] FileReceivedMessage file,
            ILogger log)
        {
            //var file = ctx.GetInput<FileReceivedMessage>();
            log.LogInformation($"Saying hello to {file.FileName}.");

            var savedFile = await dbContext.ReceivedFiles.FindAsync(file.FileName);

            if (savedFile == null)
            {
                dbContext.ReceivedFiles.Add(new ReceivedFile()
                {
                    Id = file.FileName,
                    FileStatus = file.FileExtension == "TN" ? FileStatus.DataReceivedPendingSignature : FileStatus.SignatureReceivedPendingData
                });
            }
            else
            {
                savedFile.FileStatus = FileStatus.ReceivedBoth;
            }

            dbContext.ReceivedFileHistory.Add(new ReceivedFileHistory()
            {
                ReceivedFileId = file.FileName,
                Event = file.FileExtension == "TN" ? HistoryEvent.ReceivedData : HistoryEvent.ReceivedSignature,
                TimeStamp = DateTime.Now
            });

            await dbContext.SaveChangesAsync();
        }
    }

    public class FileReceivedMessage
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public override string ToString()
        {
            return $"{FileName}.{FileExtension}";
        }
    }
}