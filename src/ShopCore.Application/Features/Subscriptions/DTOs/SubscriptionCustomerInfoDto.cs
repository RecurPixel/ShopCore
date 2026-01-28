namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionCustomerInfoDto(
    int UserId,
    string FullName,
    string? Email,
    string? Phone,
    string DeliveryAddress,
    string? DeliveryInstructions,
    decimal? Latitude,
    decimal? Longitude
);
