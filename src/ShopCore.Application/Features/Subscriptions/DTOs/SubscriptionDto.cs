using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionDto(
    int Id,
    string SubscriptionNumber,
    int UserId,
    int VendorId,
    string VendorName,
    int DeliveryAddressId,
    AddressDto DeliveryAddress,
    SubscriptionFrequency Frequency,
    int? CustomFrequencyDays,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime NextDeliveryDate,
    string? PreferredDeliveryTime,
    int BillingCycleDays,
    DateTime? NextBillingDate,
    decimal UnpaidAmount,
    decimal CreditLimit,
    decimal? DepositAmount,
    decimal? DepositPaid,
    decimal? DepositBalance,
    SubscriptionStatus Status,
    int TotalDeliveries,
    int CompletedDeliveries,
    int FailedDeliveries,
    List<SubscriptionItemDto> Items
);
