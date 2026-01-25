namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceLineItemDto(
    int Id,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
