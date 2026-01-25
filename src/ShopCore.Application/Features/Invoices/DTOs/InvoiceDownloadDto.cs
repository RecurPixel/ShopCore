namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDownloadDto(byte[] FileContent, string FileName, string ContentType);
