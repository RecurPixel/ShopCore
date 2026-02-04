namespace ShopCore.Application.Common.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateOrderInvoicePdfAsync(Order order);
    Task<byte[]> GenerateDeliveryReceiptAsync(Delivery delivery);
    Task<byte[]> GenerateSubscriptionInvoicePdfAsync(SubscriptionInvoice invoice);
}
