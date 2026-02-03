using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.UpdateSubscription;

public record UpdateSubscriptionCommand(
    int Id,
    List<SubscriptionItemDto> Items,
    SubscriptionFrequency Frequency,
    int? CustomFrequencyDays,
    string? PreferredDeliveryTime
) : IRequest<SubscriptionDto>;

public record SubscriptionItemDto
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}