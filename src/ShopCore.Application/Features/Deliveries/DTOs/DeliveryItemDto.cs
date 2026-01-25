namespace ShopCore.Application.Deliveries.DTOs;

public record DeliveryItemDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
