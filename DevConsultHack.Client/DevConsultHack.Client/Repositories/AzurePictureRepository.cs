using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevConsultHack.Client.Models;
using Newtonsoft.Json;

namespace DevConsultHack.Client.Repositories
{
    public class AzurePictureRepository : IPictureRepository
    {
        private string UploadEndpoint { get; }
        private string HistoryEndpoint { get; }

        public AzurePictureRepository(Settings settings)
        {
            var azureEndpoint = settings.AzureEndpoint.EndsWith("/") ?
                settings.AzureEndpoint :
                settings.AzureEndpoint = "/";
            UploadEndpoint = $"{azureEndpoint}api/Upload";
            HistoryEndpoint = $"{azureEndpoint}api/History";
        }

        public async Task<HistoryResult> GetHistoriesAsync(string nextToken = null)
        {
            string createParameter()
            {
                if (string.IsNullOrEmpty(nextToken))
                {
                    return ""; ;
                }

                return $"?token={HttpUtility.UrlEncode(nextToken, Encoding.UTF8)}";
            }

            using (var client = new HttpClient())
            {
                var r = await client.GetAsync($"{HistoryEndpoint}{createParameter()}");
                if (!r.IsSuccessStatusCode)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<HistoryResult>(await r.Content.ReadAsStringAsync());
            }
        }

        public async Task<UploadResult> UploadAsync(byte[] image)
        {
            using (var client = new HttpClient())
            {
                var r = await client.PostAsync(UploadEndpoint, new ByteArrayContent(image));
                if (!r.IsSuccessStatusCode)
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<UploadResult>(await r.Content.ReadAsStringAsync());
            }
        }
    }
}
