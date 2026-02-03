namespace ShopCore.Application.Common.Models;

public class FileResponse
{
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long FileSize => Content.Length;
}
