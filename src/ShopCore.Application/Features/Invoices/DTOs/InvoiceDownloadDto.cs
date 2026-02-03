namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDownloadDto
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}