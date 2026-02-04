namespace ShopCore.Application.Common.Interfaces;

/// <summary>
/// File storage service supporting multiple providers (Local, Azure Blob, etc.)
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Upload a file from IFile (ASP.NET Core file upload)
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="fileName">Target file name/path (e.g., "products/123/image.jpg")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Public URL of the uploaded file</returns>
    Task<string> UploadFileAsync(IFile file, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload a file from a stream
    /// </summary>
    /// <param name="stream">File content stream</param>
    /// <param name="fileName">Target file name/path</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Public URL of the uploaded file</returns>
    Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file by its path/key
    /// </summary>
    /// <param name="filePath">File path or key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a file as a stream
    /// </summary>
    /// <param name="filePath">File path or key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File content stream</returns>
    Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the public URL for a file
    /// </summary>
    /// <param name="filePath">File path or key</param>
    /// <returns>Public URL</returns>
    string GetFileUrl(string filePath);

    /// <summary>
    /// Check if a file exists
    /// </summary>
    /// <param name="filePath">File path or key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if file exists</returns>
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
}
