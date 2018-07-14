
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using DevConsultHack.Server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Linq;

namespace DevConsultHack.Server
{
    public static class Upload
    {
        [FunctionName(nameof(Upload))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, 
            IBinder binder,
            [Table(Consts.TransactionsTableName)] IAsyncCollector<Transaction> transactions,
            ILogger log,
            ExecutionContext context)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation("Upload started");

            var ms = new MemoryStream();
            await req.Body.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var buffer = ms.ToArray();
            var emotion = await DetectEmotionAsync(buffer, config.GetSection("FaceAPI"));

            if (string.IsNullOrEmpty(emotion))
            {
                return new JsonResult(new UploadResult
                {
                    Timestamp = DateTimeOffset.UtcNow,
                });
            }

            log.LogInformation("Copy to blob storage starting");
            var blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute($"photos/{Guid.NewGuid()}.png", FileAccess.Write));
            await blob.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
            log.LogInformation("Copy to blob storage finished");

            log.LogInformation("Add transaction");
            await transactions.AddAsync(new Transaction
            {
                PartitionKey = "DevConsult",
                RowKey = Guid.NewGuid().ToString(),
                Path = blob.Uri.ToString(),
                Emotion = emotion,
            });

            return new JsonResult(new UploadResult
            {
                Emotion = emotion,
                Timestamp = DateTimeOffset.UtcNow,
            });
        }

        private static async Task<string> DetectEmotionAsync(byte[] image, IConfigurationSection faceAPISettings)
        {
            var client = new FaceClient(new ApiKeyServiceClientCredentials(faceAPISettings["Key"]));
            client.BaseUri = new Uri(faceAPISettings["Endpoint"]);
            var result = await client.Face.DetectWithStreamAsync(new MemoryStream(image), returnFaceAttributes: new List<FaceAttributeType>
            {
                FaceAttributeType.Emotion,
            });

            if (!result.Any())
            {
                return null;
            }

            var emotion = result.OrderByDescending(x => x.FaceRectangle.Width * x.FaceRectangle.Height)
                .First()
                .FaceAttributes
                .Emotion;
            return typeof(Emotion)
                .GetProperties()
                .Select(x => (name: x.Name, value: x.GetValue(emotion)))
                .OrderByDescending(x => x.value)
                .First()
                .name;
        }
    }
}
