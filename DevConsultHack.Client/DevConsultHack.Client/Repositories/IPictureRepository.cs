using DevConsultHack.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevConsultHack.Client.Repositories
{
    public interface IPictureRepository
    {
        Task<HistoryResult> GetHistoriesAsync(string nextToken = null);
        Task<UploadResult> UploadAsync(byte[] image);
    }
}
