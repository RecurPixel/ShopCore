namespace ShopCore.Infrastructure.FileStorage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public FileStorageProvider Provider { get; set; } = FileStorageProvider.Local;
    public LocalStorageOptions Local { get; set; } = new();
    public AzureBlobOptions AzureBlob { get; set; } = new();
    public S3Options S3 { get; set; } = new();
}

public enum FileStorageProvider
{
    Local = 0,
    AzureBlob = 1,
    S3 = 2,
    Storj = 3
}

public class LocalStorageOptions
{
    /// <summary>
    /// Root directory for file storage (relative to application root or absolute path)
    /// </summary>
    public string RootPath { get; set; } = "wwwroot/uploads";

    /// <summary>
    /// Base URL for accessing files (used to generate public URLs)
    /// </summary>
    public string BaseUrl { get; set; } = "/uploads";

    /// <summary>
    /// Create directories if they don't exist
    /// </summary>
    public bool AutoCreateDirectories { get; set; } = true;
}

public class AzureBlobOptions
{
    /// <summary>
    /// Azure Storage connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Container name for storing files
    /// </summary>
    public string ContainerName { get; set; } = "shopcore-files";

    /// <summary>
    /// Optional CDN endpoint URL (e.g., https://shopcore.azureedge.net)
    /// </summary>
    public string? CdnEndpoint { get; set; }

    /// <summary>
    /// Public access level for the container
    /// </summary>
    public bool EnablePublicAccess { get; set; } = true;
}

public class S3Options
{
    /// <summary>
    /// Custom endpoint URL for S3-compatible providers.
    /// Leave empty for AWS S3. Set for Cloudflare R2, Backblaze B2, MinIO, Storj DCS, etc.
    /// Examples:
    ///   Cloudflare R2: https://&lt;account-id&gt;.r2.cloudflarestorage.com
    ///   Storj DCS:     https://gateway.storjshare.io
    ///   MinIO:         http://localhost:9000
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "shopcore-files";

    /// <summary>
    /// AWS region. Use "auto" for Cloudflare R2 or "us-east-1" for other S3-compatible services.
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Public base URL for generating file URLs.
    /// Can be a CDN domain (e.g., https://cdn.example.com) or the bucket public URL.
    /// If empty, URLs are constructed from Endpoint + BucketName.
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Use path-style URLs (bucket in path). Required for MinIO, Storj, and most non-AWS providers.
    /// AWS virtual-hosted style: https://bucket.s3.amazonaws.com/key
    /// Path style:               https://s3.amazonaws.com/bucket/key
    /// </summary>
    public bool ForcePathStyle { get; set; } = true;
}
