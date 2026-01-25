namespace ShopCore.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<Stream> GetFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
