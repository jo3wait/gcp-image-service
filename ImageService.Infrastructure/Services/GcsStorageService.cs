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
        // Service Account �w�[�W storage.objectAdmin�]�� storage.objects.create + storage.objects.get�^
        // StorageClient.Create() �|�۰ʥιw�]���� (ADC) �h�I�s GCS API
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

        // �^��name
        return name;
    }

    public string GetDownloadUrl(string objectPath, TimeSpan _)
    {
        // �Y����]�w���}Ū���A�����^�� URL
        return $"https://storage.googleapis.com/{_bucket}/{objectPath}";
    }
}