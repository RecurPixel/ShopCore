using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Infrastructure.FileStorage;

public class S3FileStorageService : IFileStorageService
{
    private readonly S3Options _options;
    private readonly IAmazonS3 _s3Client;
    private readonly string _publicBaseUrl;

    public S3FileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value.S3;

        var config = new AmazonS3Config
        {
            ForcePathStyle = _options.ForcePathStyle
        };

        if (!string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            config.ServiceURL = _options.Endpoint;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
        }

        var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
        _s3Client = new AmazonS3Client(credentials, config);

        _publicBaseUrl = ResolvePublicBaseUrl();
    }

    public async Task<string> UploadFileAsync(IFile file, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = file.OpenReadStream();
        return await UploadFileAsync(stream, fileName, file.ContentType, cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var key = SanitizeKey(fileName);

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return GetFileUrl(key);
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var key = SanitizeKey(filePath);

        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var key = SanitizeKey(filePath);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException($"File not found in S3: {filePath}");
        }
    }

    public string GetFileUrl(string filePath)
    {
        var key = SanitizeKey(filePath);
        return $"{_publicBaseUrl}/{key}";
    }

    public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var key = SanitizeKey(filePath);

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private string ResolvePublicBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            return _options.PublicBaseUrl.TrimEnd('/');

        // Build from endpoint + bucket (path-style) or AWS virtual-hosted style
        if (!string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            var endpoint = _options.Endpoint.TrimEnd('/');
            return _options.ForcePathStyle
                ? $"{endpoint}/{_options.BucketName}"
                : $"https://{_options.BucketName}.{endpoint.Replace("https://", "").Replace("http://", "")}";
        }

        // Standard AWS path
        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com";
    }

    private static string SanitizeKey(string key)
    {
        // Remove path traversal attempts
        key = key.Replace("..", string.Empty);

        // Normalize to forward slashes (S3 uses / as separator)
        key = key.Replace("\\", "/");

        // Remove leading slashes
        key = key.TrimStart('/');

        return key;
    }
}
