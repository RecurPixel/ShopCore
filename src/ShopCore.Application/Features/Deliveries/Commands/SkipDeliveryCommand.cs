namespace ShopCore.Application.Deliveries.Commands.SkipDelivery;

public record SkipDeliveryCommand(
    int Id,
    string? Reason
) : IRequest<DeliveryDto>;
