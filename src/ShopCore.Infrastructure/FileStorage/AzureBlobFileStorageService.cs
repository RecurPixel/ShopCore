using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Infrastructure.FileStorage;

public class AzureBlobFileStorageService : IFileStorageService
{
    private readonly AzureBlobOptions _options;
    private readonly BlobContainerClient _containerClient;
    private readonly string _baseUrl;

    public AzureBlobFileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value.AzureBlob;

        // Create blob service client
        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);

        // Get or create container
        _containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        _containerClient.CreateIfNotExists();

        // Set public access if enabled
        if (_options.EnablePublicAccess)
        {
            _containerClient.SetAccessPolicy(PublicAccessType.Blob);
        }

        // Determine base URL (CDN or direct blob URL)
        _baseUrl = !string.IsNullOrEmpty(_options.CdnEndpoint)
            ? _options.CdnEndpoint.TrimEnd('/')
            : _containerClient.Uri.ToString().TrimEnd('/');
    }

    public async Task<string> UploadFileAsync(IFile file, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = file.OpenReadStream();
        return await UploadFileAsync(stream, fileName, file.ContentType, cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        // Sanitize the blob name
        var blobName = SanitizeBlobName(fileName);

        // Get blob client
        var blobClient = _containerClient.GetBlobClient(blobName);

        // Set upload options
        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };

        // Upload the blob
        await blobClient.UploadAsync(stream, uploadOptions, cancellationToken);

        // Return the public URL
        return GetFileUrl(blobName);
    }

    public async Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var blobName = SanitizeBlobName(filePath);
        var blobClient = _containerClient.GetBlobClient(blobName);

        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var blobName = SanitizeBlobName(filePath);
        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException($"Blob not found: {filePath}");
        }

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public string GetFileUrl(string filePath)
    {
        var blobName = SanitizeBlobName(filePath);

        // Normalize path separators for URLs
        var urlPath = blobName.Replace("\\", "/");

        return $"{_baseUrl}/{urlPath}";
    }

    public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var blobName = SanitizeBlobName(filePath);
        var blobClient = _containerClient.GetBlobClient(blobName);

        return await blobClient.ExistsAsync(cancellationToken);
    }

    private static string SanitizeBlobName(string blobName)
    {
        // Remove any leading path traversal attempts
        blobName = blobName.Replace("..", string.Empty);

        // Normalize to forward slashes (Azure Blob uses / as separator)
        blobName = blobName.Replace("\\", "/");

        // Remove leading slashes
        blobName = blobName.TrimStart('/');

        return blobName;
    }
}
