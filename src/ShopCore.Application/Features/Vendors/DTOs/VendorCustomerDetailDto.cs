namespace ShopCore.Application.Vendors.DTOs;

public record VendorCustomerDetailDto(
    int UserId,
    string FullName,
    string? Email,
    string? Phone,
    string? DefaultAddress,
    int ActiveSubscriptionCount,
    int TotalOrderCount,
    int TotalDeliveryCount,
    decimal TotalSpent,
    DateTime? FirstOrderDate,
    DateTime? LastOrderDate,
    DateTime? LastDeliveryDate,
    string? Notes
);
