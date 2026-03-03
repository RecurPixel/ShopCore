namespace ShopCore.Application.Common.Interfaces;

public interface INotificationService
{
    // Auth
    Task SendWelcomeAsync(User user);
    Task SendEmailVerificationAsync(User user, string token, string verifyUrl);
    Task SendPasswordResetAsync(User user, string token, string resetUrl);

    // Orders
    Task SendOrderPlacedAsync(User user, int orderId, string orderNumber, decimal total);
    Task SendOrderCancelledAsync(User user, int orderId, string orderNumber, string? reason);
    Task SendRefundProcessedAsync(User user, int orderId, decimal refundAmount);

    // Subscriptions & Deliveries
    Task SendSubscriptionCreatedAsync(User user, int subscriptionId, string subscriptionNumber);
    Task SendDeliverySkippedAsync(User user, int deliveryId);

    // Invoices
    Task SendInvoicePaidAsync(User user, string invoiceNumber, decimal total);

    // Vendor
    Task SendVendorApprovedAsync(User user);
    Task SendVendorSuspendedAsync(User user);

    // Payouts
    Task SendPayoutProcessedAsync(User user, decimal netAmount);
}
