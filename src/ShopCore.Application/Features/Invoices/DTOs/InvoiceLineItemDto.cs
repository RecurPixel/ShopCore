namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceLineItemDto(
    int Id,
    int ProductId,
    string ProductName,
    DateTime DeliveryDate,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
