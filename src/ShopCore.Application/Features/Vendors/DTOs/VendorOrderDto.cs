namespace ShopCore.Application.Vendors.DTOs;

public record VendorOrderDto(
    int Id,
    string OrderNumber,
    int CustomerId,
    string CustomerName,
    OrderStatus Status,
    decimal SubTotal,
    decimal Total,
    DateTime OrderDate,
    string? Notes,
    List<VendorOrderItemDto> Items
);
