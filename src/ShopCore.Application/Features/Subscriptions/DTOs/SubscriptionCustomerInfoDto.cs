namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionCustomerInfoDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string DeliveryAddress { get; init; } = string.Empty;
    public string? DeliveryInstructions { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}