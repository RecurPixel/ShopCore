namespace ShopCore.Domain.Enums;

public enum NotificationType
{
    // Auth
    Welcome,
    EmailVerification,
    PasswordReset,

    // Orders
    OrderPlaced,
    OrderStatusChanged,
    OrderCancelled,
    RefundProcessed,

    // Subscriptions & Deliveries
    SubscriptionCreated,
    DeliverySkipped,
    DeliveryCompleted,

    // Payments & Invoices
    InvoicePaid,
    PaymentFailed,

    // Vendor
    VendorApproved,
    VendorSuspended,
    PayoutProcessed,

    // General
    System
}
