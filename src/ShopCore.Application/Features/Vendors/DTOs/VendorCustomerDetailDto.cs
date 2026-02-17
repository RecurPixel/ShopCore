namespace ShopCore.Application.Vendors.DTOs;

public record VendorCustomerDetailDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? DefaultAddress { get; init; }
    public int ActiveSubscriptionCount { get; init; }
    public int TotalOrderCount { get; init; }
    public int TotalDeliveryCount { get; init; }
    public decimal TotalSpent { get; init; }
    public DateTime? FirstOrderDate { get; init; }
    public DateTime? LastOrderDate { get; init; }
    public DateTime? LastDeliveryDate { get; init; }
    public string? Notes { get; init; }
}