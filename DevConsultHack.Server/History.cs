
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using DevConsultHack.Server.Models;

namespace DevConsultHack.Server
{
    public static class History
    {
        [FunctionName(nameof(History))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, 
            [Table(Consts.TransactionsTableName)] CloudTable transactions,
            ILogger log)
        {
            string token = null;
            if (req.Query.TryGetValue("token", out var tokenValues))
            {
                token = tokenValues.First();
            }

            var tableContinuationToken = string.IsNullOrEmpty(token) ?
                null :
                JsonConvert.DeserializeObject<TableContinuationToken>(token);
            var q = new TableQuery<Transaction>()
                .Where(TableQuery.GenerateFilterCondition(nameof(Transaction.PartitionKey), QueryComparisons.Equal, "DevConsult"));
            var results = await transactions.ExecuteQuerySegmentedAsync<Transaction>(q, tableContinuationToken);
            return new JsonResult(new HistoryResult
            {
                Transactions = results.ToArray(),
                NextToken = results.ContinuationToken == null ? 
                    null : 
                    JsonConvert.SerializeObject(results.ContinuationToken),
            });
        }
    }
}
