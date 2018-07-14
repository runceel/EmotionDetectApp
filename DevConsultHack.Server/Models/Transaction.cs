using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevConsultHack.Server.Models
{
    public class Transaction : TableEntity
    {
        public string Path { get; set; }
        public string Emotion { get; set; }
    }
}
