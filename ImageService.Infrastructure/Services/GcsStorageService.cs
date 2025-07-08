using Google.Cloud.Storage.V1;
using ImageService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Services;

public class GcsStorageService : IStorageService
{
    private readonly StorageClient _client;
    private readonly string _bucket;

    public GcsStorageService(IConfiguration cfg)
    {
        // Service Account 已加上 storage.objectAdmin（或 storage.objects.create + storage.objects.get）
        // StorageClient.Create() 會自動用預設憑證 (ADC) 去呼叫 GCS API
        _client = StorageClient.Create();
        _bucket = cfg["GoogleCloud:BucketName"]!;
    }

    public async Task<string> UploadAsync(string name, Stream data, string contentType)
    {
        await _client.UploadObjectAsync(
            bucket: _bucket,
            objectName: name,
            contentType: contentType,
            source: data/*,
            options: new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            }*/);

        // 回傳name
        return name;
    }

    public string GetDownloadUrl(string objectPath, TimeSpan _)
    {
        // 若物件設定公開讀取，直接回傳 URL
        return $"https://storage.googleapis.com/{_bucket}/{objectPath}";
    }
}