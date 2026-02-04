namespace ShopCore.Infrastructure.FileStorage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public FileStorageProvider Provider { get; set; } = FileStorageProvider.Local;
    public LocalStorageOptions Local { get; set; } = new();
    public AzureBlobOptions AzureBlob { get; set; } = new();
}

public enum FileStorageProvider
{
    Local = 0,
    AzureBlob = 1
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
