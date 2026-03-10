namespace ShopCore.Application.Deliveries.Queries.GetDeliveryReceipt;

public record GetDeliveryReceiptQuery(int DeliveryId) : IRequest<FileResponse>;
