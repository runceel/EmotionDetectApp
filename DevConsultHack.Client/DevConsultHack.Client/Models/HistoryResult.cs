using System;
using System.Collections.Generic;
using System.Text;

namespace DevConsultHack.Client.Models
{
    public class HistoryResult
    {
        public Transaction[] Transactions { get; set; }
        public string NextToken { get; set; }
    }
}
