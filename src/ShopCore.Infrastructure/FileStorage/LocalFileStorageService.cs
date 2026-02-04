using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly LocalStorageOptions _options;
    private readonly string _rootPath;

    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value.Local;

        // Convert relative path to absolute if needed
        _rootPath = Path.IsPathRooted(_options.RootPath)
            ? _options.RootPath
            : Path.Combine(Directory.GetCurrentDirectory(), _options.RootPath);

        // Create root directory if it doesn't exist
        if (_options.AutoCreateDirectories && !Directory.Exists(_rootPath))
        {
            Directory.CreateDirectory(_rootPath);
        }
    }

    public async Task<string> UploadFileAsync(IFile file, string fileName, CancellationToken cancellationToken = default)
    {
        using var stream = file.OpenReadStream();
        return await UploadFileAsync(stream, fileName, file.ContentType, cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        // Sanitize the file name to prevent directory traversal
        var sanitizedFileName = SanitizeFileName(fileName);
        var fullPath = Path.Combine(_rootPath, sanitizedFileName);

        // Ensure directory exists
        var directory = Path.GetDirectoryName(fullPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Save the file
        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);

        // Return the public URL
        return GetFileUrl(sanitizedFileName);
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var sanitizedPath = SanitizeFileName(filePath);
        var fullPath = Path.Combine(_rootPath, sanitizedPath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var sanitizedPath = SanitizeFileName(filePath);
        var fullPath = Path.Combine(_rootPath, sanitizedPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult(stream);
    }

    public string GetFileUrl(string filePath)
    {
        var sanitizedPath = SanitizeFileName(filePath);

        // Normalize path separators for URLs
        var urlPath = sanitizedPath.Replace("\\", "/");

        // Ensure base URL ends with / and file path doesn't start with /
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var normalizedPath = urlPath.TrimStart('/');

        return $"{baseUrl}/{normalizedPath}";
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var sanitizedPath = SanitizeFileName(filePath);
        var fullPath = Path.Combine(_rootPath, sanitizedPath);
        return Task.FromResult(File.Exists(fullPath));
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove any leading path traversal attempts
        fileName = fileName.Replace("..", string.Empty);

        // Normalize path separators
        fileName = fileName.Replace("/", Path.DirectorySeparatorChar.ToString());

        // Remove any leading directory separator
        fileName = fileName.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return fileName;
    }
}
