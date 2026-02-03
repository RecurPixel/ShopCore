namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceLineItemDto
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
