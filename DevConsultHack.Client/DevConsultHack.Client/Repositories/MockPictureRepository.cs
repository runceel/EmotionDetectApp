using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevConsultHack.Client.Models;

namespace DevConsultHack.Client.Repositories
{
    class MockPictureRepository : IPictureRepository
    {
        public Task<HistoryResult> GetHistoriesAsync(string nextToken = null)
        {
            return Task.FromResult(new HistoryResult
            {
                Transactions = new[]
                {
                    new Transaction
                    {
                        Path = "https://pbs.twimg.com/profile_images/936194790110011393/796OQ9nn_400x400.jpg",
                        Emotion = "Happiness",
                        Timestamp = DateTimeOffset.Now,
                    },
                    new Transaction
                    {
                        Path = "https://pbs.twimg.com/profile_images/936194790110011393/796OQ9nn_400x400.jpg",
                        Emotion = "Happiness",
                        Timestamp = DateTimeOffset.Now,
                    },
                    new Transaction
                    {
                        Path = "https://pbs.twimg.com/profile_images/936194790110011393/796OQ9nn_400x400.jpg",
                        Emotion = "Happiness",
                        Timestamp = DateTimeOffset.Now,
                    },
                    new Transaction
                    {
                        Path = "https://pbs.twimg.com/profile_images/936194790110011393/796OQ9nn_400x400.jpg",
                        Emotion = "Happiness",
                        Timestamp = DateTimeOffset.Now,
                    },
                    new Transaction
                    {
                        Path = "https://pbs.twimg.com/profile_images/936194790110011393/796OQ9nn_400x400.jpg",
                        Emotion = "Happiness",
                        Timestamp = DateTimeOffset.Now,
                    },
                },
                NextToken = "mockNextToken",
            });
        }

        public Task<UploadResult> UploadAsync(byte[] image)
        {
            return Task.FromResult(new UploadResult
            {
                Emotion = "Happiness",
                Timestamp = DateTimeOffset.Now,
            });
        }
    }
}
