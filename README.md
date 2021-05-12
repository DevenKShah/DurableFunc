# DurableFunc

## Configuration
Add local.settings.json 
``` json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "SqlConnectionString": "<connection string>"
}
```

## Setup
### Storage account
create a queue `filerecevied-queue`

### SQL Server
Create tables
``` sql
create table ReceivedFiles (
  Id VARCHAR(50) PRIMARY KEY,
  FileStatus VARCHAR(50) NOT NULL
);

create table ReceivedFileHistory (
  Id UNIQUEIDENTIFIER PRIMARY KEY,
  ReceivedFileId VARCHAR(50) NOT NULL,
  TimeStamp DATETIME NOT NULL,
  Event VARCHAR(50) NOT NULL
);
```

## To Debug
Drop a message on the queue. Below is a sample c# code on how to drop the message
``` c#
private static void SendMessage()
{
	string connectionString = "UseDevelopmentStorage=true";
	var queueClient = new QueueClient(connectionString, "filereceived-queue");
  
  var filename = Guid.NewGuid().ToString();
	var msg1 = JsonSerializer.Serialize(new FileReceivedMessage(filename, "TN"));
  queueClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(msg1)));
 }
 
class FileReceivedMessage
{
	public string FileName { get; set; }
	public string FileExtension { get; set; }
	public FileReceivedMessage(string fileName, string fileExtension)
	{
		FileName = fileName;
		FileExtension = fileExtension;
	}
}
```
