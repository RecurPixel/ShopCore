namespace ShopCore.Application.Vendors.DTOs;

public record VendorCustomerDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public int ActiveSubscriptionCount { get; init; }
    public int TotalOrderCount { get; init; }
    public DateTime? LastOrderDate { get; init; }
    public DateTime? LastDeliveryDate { get; init; }
}