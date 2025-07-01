using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageService.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadAsync(string fileName, Stream data, string contentType);

        /* 取得下載連結，ttl = 有效期 */
        string GetDownloadUrl(string objectPath, TimeSpan ttl);
    }
}
