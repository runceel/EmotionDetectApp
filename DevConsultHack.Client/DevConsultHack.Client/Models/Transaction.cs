using System;
using System.Collections.Generic;
using System.Text;

namespace DevConsultHack.Client.Models
{
    public class Transaction
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Path { get; set; }
        public string Emotion { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
