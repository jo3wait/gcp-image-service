using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using ImageService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace ImageService.Infrastructure.Services;

public class GcsStorageService : IStorageService
{
    private readonly StorageClient _client = StorageClient.Create();
    private readonly string _bucket;
    private readonly UrlSigner _signer;

    public GcsStorageService(IConfiguration cfg)
    {
        _bucket = cfg["GoogleCloud:BucketName"]!;
        _client = StorageClient.Create();

        // 取得目前執行環境 (Cloud Run SA) 的憑證作 UrlSigner
        var cred = GoogleCredential.GetApplicationDefault()
                        .UnderlyingCredential as ServiceAccountCredential
                   ?? throw new InvalidOperationException("Need service-account credential");
        _signer = UrlSigner.FromCredential(cred);
    }
    
    public async Task<string> UploadAsync(string name, Stream data, string contentType)
    {
        await _client.UploadObjectAsync(_bucket, name, contentType, data);
        return name;                 // 只回傳 GCS 物件路徑 (不含 bucket)
    }

    public string GetDownloadUrl(string objectPath, TimeSpan ttl)
    {
        return _signer.Sign(
            _bucket,
            objectPath,
            ttl,
            HttpMethod.Get);
    }
}