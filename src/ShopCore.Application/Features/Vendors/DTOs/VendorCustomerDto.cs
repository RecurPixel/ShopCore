namespace ShopCore.Application.Vendors.DTOs;

public record VendorCustomerDto(
    int UserId,
    string FullName,
    string? Email,
    string? Phone,
    int ActiveSubscriptionCount,
    int TotalOrderCount,
    DateTime? LastOrderDate,
    DateTime? LastDeliveryDate
);
