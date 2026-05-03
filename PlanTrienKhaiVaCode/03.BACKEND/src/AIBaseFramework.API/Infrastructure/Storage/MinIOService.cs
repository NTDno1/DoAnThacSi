// MinIO Storage Service
using Minio;
using Minio.DataModel.Args;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Infrastructure.Storage;

public interface IMinIOService
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string objectName);
    Task<void> DeleteFileAsync(string objectName);
    Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 60);
    Task EnsureBucketExistsAsync(string bucketName);
}

public class MinIOService : IMinIOService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinIOService> _logger;
    private readonly string _bucketName;

    public MinIOService(
        IMinioClient minioClient,
        IConfiguration configuration,
        ILogger<MinIOService> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
        _bucketName = configuration["MinIOConfig:BucketName"] ?? "documents";
    }

    public async Task EnsureBucketExistsAsync(string bucketName)
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
                _logger.LogInformation("Created bucket: {Bucket}", bucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure bucket exists: {Bucket}", bucketName);
            throw;
        }
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType)
    {
        try
        {
            var objectName = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}/{fileName}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            _logger.LogInformation("Uploaded file: {ObjectName}", objectName);
            return objectName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string objectName)
    {
        try
        {
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => 
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await _minioClient.GetObjectAsync(getObjectArgs);

            return memoryStream;
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
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);
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
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithExpiry(expiryMinutes * 60);

            var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned URL: {ObjectName}", objectName);
            throw;
        }
    }
}
