namespace ShopCore.Application.Vendors.DTOs;

public record VendorOrderItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
