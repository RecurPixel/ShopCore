using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionDto
{
    public int Id { get; init; }
    public string SubscriptionNumber { get; init; } = string.Empty;
    public int UserId { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public int DeliveryAddressId { get; init; }
    public AddressDto? DeliveryAddress { get; init; }
    public string Frequency { get; init; } = string.Empty;
    public int? CustomFrequencyDays { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public DateTime NextDeliveryDate { get; init; }
    public string? PreferredDeliveryTime { get; init; }
    public int BillingCycleDays { get; init; }
    public DateTime? NextBillingDate { get; init; }
    public decimal UnpaidAmount { get; init; }
    public decimal CreditLimit { get; init; }
    public decimal? DepositAmount { get; init; }
    public decimal? DepositPaid { get; init; }
    public decimal? DepositBalance { get; init; }
    public string Status { get; init; } = string.Empty;
    public int TotalDeliveries { get; init; }
    public int CompletedDeliveries { get; init; }
    public int FailedDeliveries { get; init; }
    public int ItemCount { get; init; }
    public List<SubscriptionItemDto> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
