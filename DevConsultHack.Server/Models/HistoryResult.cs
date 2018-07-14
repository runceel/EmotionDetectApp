using System;
using System.Collections.Generic;
using System.Text;

namespace DevConsultHack.Server.Models
{
    public class HistoryResult
    {
        public Transaction[] Transactions { get; set; }
        public string NextToken { get; set; }
    }
}
