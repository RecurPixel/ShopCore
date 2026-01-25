namespace ShopCore.Application.Common.Interfaces;

public interface IFile
{
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }

    Stream OpenReadStream();
}
