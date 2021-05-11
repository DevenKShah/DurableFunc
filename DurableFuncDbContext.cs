using System;
using Microsoft.EntityFrameworkCore;

namespace DurableFunc
{

    public class DurableFuncDbContext : DbContext
    {
        public DurableFuncDbContext(DbContextOptions<DurableFuncDbContext> options) : base(options) { }

        public DbSet<ReceivedFile> ReceivedFiles { get; set; }
        public DbSet<ReceivedFileHistory> ReceivedFileHistory { get; set; }
    }
    public class ReceivedFile
    {
        public string Id { get; set; }
        public string FileStatus {get; set;}
    }

    public class ReceivedFileHistory
    {
        public Guid Id {get; set;}  = Guid.NewGuid();
        public string ReceivedFileId {get; set;}
        public DateTime TimeStamp {get;set;}
        public string Event {get;set;}
    }

    public static class HistoryEvent
    {
        public const string ReceivedData = "ReceivedData";
        public const string ReceivedSignature = "ReceivedSignature";
    }

    public static class FileStatus
    {
        public const string DataReceivedPendingSignature = "DataReceivedPendingSignature";
        public const string SignatureReceivedPendingData = "SignatureReceivedPendingData";
        public const string ReceivedBoth = "ReceivedBoth";
    }
}