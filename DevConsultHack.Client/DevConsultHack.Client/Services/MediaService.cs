using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevConsultHack.Client.Services
{
    public class MediaService : IMediaService
    {
        public async Task<byte[]> TakePhotoAsync()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsTakePhotoSupported)
            {
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
            if (file == null)
            {
                return null;
            }

            using (var s = file.GetStream())
            using (var ms = new MemoryStream())
            {
                await s.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
