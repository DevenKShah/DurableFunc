/*
 delete from ReceivedFiles
 delete from ReceivedFileHistory

 drop table ReceivedFiles
 drop table ReceivedFileHistory
*/

create database DurableFuncDb;

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
