using ImageService.Application.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
namespace ImageService.Infrastructure.Services;
public class FakeStorageService : IStorageService
{
    public Task<string> UploadAsync(string name, Stream _, string __)
        => Task.FromResult($"fake://local/{Guid.NewGuid()}/{name}");

    public string GetDownloadUrl(string objectPath, TimeSpan _) =>
        $"http://localhost/fake/{objectPath}";
}