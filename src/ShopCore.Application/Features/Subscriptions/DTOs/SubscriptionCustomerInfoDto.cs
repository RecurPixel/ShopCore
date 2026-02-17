namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionCustomerInfoDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string DeliveryAddress { get; init; } = string.Empty;
    public string? DeliveryInstructions { get; init; }
    public string? PreferredDeliveryTime { get; set; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}