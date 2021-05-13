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

`KickOffDurableOrchestrationFunction` is a Http triggered function that allows you to drop messages on the queue. Each of these messages will trigger the orchestrator function. 
The route to the http trigger is `/api/sendmessages/{noOfMsgs}`, where you need to replace `{noOfMsgs}` with number of messages you would like to send. For example `/api/sendmessages/2`. 
