using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevConsultHack.Client.Services
{
    public interface IMediaService
    {
        Task<byte[]> TakePhotoAsync();
    }
}
