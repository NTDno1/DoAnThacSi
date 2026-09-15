// MinIO Storage Service
using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Infrastructure.Storage;

public interface IMinIOService
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType);
    Task<byte[]> DownloadFileAsync(string objectName);
    Task DeleteFileAsync(string objectName);
    Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 60);
}

public class MinIOService : IMinIOService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinIOService> _logger;
    private readonly string _bucketName;

    public MinIOService(
        IMinioClient minioClient,
        ILogger<MinIOService> logger,
        AIPlatformConfig config)
    {
        _minioClient = minioClient;
        _logger = logger;
        _bucketName = config.MinIO.BucketName;
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType)
    {
        try
        {
            var objectName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}/{fileName}";

            await _minioClient.PutObjectAsync(
                _bucketName,
                objectName,
                stream,
                stream.Length,
                contentType);

            _logger.LogInformation("Uploaded file: {ObjectName}", objectName);
            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<byte[]> DownloadFileAsync(string objectName)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(_bucketName, objectName, stream =>
            {
                stream.CopyTo(memoryStream);
            });
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file: {ObjectName}", objectName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string objectName)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(_bucketName, objectName);
            _logger.LogInformation("Deleted file: {ObjectName}", objectName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {ObjectName}", objectName);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 60)
    {
        try
        {
            var url = await _minioClient.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectName)
                    .WithExpiry(expiryMinutes * 60));
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned URL: {ObjectName}", objectName);
            throw;
        }
    }
}
