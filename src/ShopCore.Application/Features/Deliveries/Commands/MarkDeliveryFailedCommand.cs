namespace ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;

public record MarkDeliveryFailedCommand(
    int DeliveryId,
    string Reason
) : IRequest;
