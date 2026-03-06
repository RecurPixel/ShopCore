using Microsoft.Extensions.Logging;
using RecurPixel.Notify;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotifyService _notify;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotifyService notify,
        ILogger<NotificationService> logger)
    {
        _notify = notify;
        _logger = logger;
    }

    // ── Auth ──────────────────────────────────────────────────────────────────

    public Task SendWelcomeAsync(User user) =>
        TriggerAsync("auth.welcome", user,
            emailSubject: "Welcome to ShopCore!",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Welcome to ShopCore, {user.FirstName}!</h2>
                <p>Thank you for registering. Start exploring products from local vendors near you.</p>
                <p>The ShopCore Team</p>
                </body></html>
                """,
            inAppTitle: "Welcome to ShopCore!",
            inAppMessage: $"Hi {user.FirstName}, your account has been created successfully.",
            type: NotificationType.Welcome);

    public Task SendEmailVerificationAsync(User user, string token, string verifyUrl) =>
        TriggerAsync("auth.verify-email", user,
            emailSubject: "Verify Your Email Address",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Verify Your Email</h2>
                <p>Click below to activate your account:</p>
                <p><a href="{verifyUrl}?token={token}"
                   style="background:#2196F3;color:white;padding:10px 20px;text-decoration:none;border-radius:5px">
                   Verify Email</a></p>
                <p>This link expires in 24 hours.</p>
                </body></html>
                """);

    public Task SendPasswordResetAsync(User user, string token, string resetUrl) =>
        TriggerAsync("auth.password-reset", user,
            emailSubject: "Reset Your Password",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Password Reset Request</h2>
                <p><a href="{resetUrl}?token={token}"
                   style="background:#4CAF50;color:white;padding:10px 20px;text-decoration:none;border-radius:5px">
                   Reset Password</a></p>
                <p>This link expires in 1 hour. If you did not request this, ignore this email.</p>
                </body></html>
                """);

    // ── Orders ────────────────────────────────────────────────────────────────

    public Task SendOrderPlacedAsync(User user, int orderId, string orderNumber, decimal total) =>
        TriggerAsync("order.placed", user,
            emailSubject: $"Order Confirmed – {orderNumber}",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Order Confirmed!</h2>
                <p>Your order <strong>{orderNumber}</strong> has been confirmed.</p>
                <p>Total: <strong>₹{total:N2}</strong></p>
                <p>You will receive updates as your order is processed.</p>
                </body></html>
                """,
            inAppTitle: "Order Placed",
            inAppMessage: $"Your order #{orderNumber} (₹{total:N2}) has been placed successfully.",
            type: NotificationType.OrderPlaced,
            referenceId: orderId,
            referenceType: "Order");

    public Task SendOrderCancelledAsync(User user, int orderId, string orderNumber, string? reason)
    {
        var reasonText = string.IsNullOrWhiteSpace(reason) ? string.Empty : $" Reason: {reason}";
        return TriggerAsync("order.cancelled", user,
            emailSubject: $"Order Cancelled – {orderNumber}",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Order Cancelled</h2>
                <p>Your order <strong>{orderNumber}</strong> has been cancelled.{reasonText}</p>
                <p>If you believe this is a mistake, please contact support.</p>
                </body></html>
                """,
            inAppTitle: "Order Cancelled",
            inAppMessage: $"Your order #{orderNumber} has been cancelled.{reasonText}",
            type: NotificationType.OrderCancelled,
            referenceId: orderId,
            referenceType: "Order");
    }

    public Task SendRefundProcessedAsync(User user, int orderId, decimal refundAmount) =>
        TriggerAsync("order.refund", user,
            emailSubject: "Refund Initiated",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Refund Initiated</h2>
                <p>A refund of <strong>₹{refundAmount:N2}</strong> has been initiated.</p>
                <p>It may take 5–7 business days to reflect in your account.</p>
                </body></html>
                """,
            inAppTitle: "Refund Initiated",
            inAppMessage: $"A refund of ₹{refundAmount:N2} has been initiated for your order.",
            type: NotificationType.RefundProcessed,
            referenceId: orderId,
            referenceType: "Order");

    // ── Subscriptions & Deliveries ────────────────────────────────────────────

    public Task SendSubscriptionCreatedAsync(User user, int subscriptionId, string subscriptionNumber) =>
        TriggerAsync("subscription.created", user,
            emailSubject: $"Subscription Confirmed – {subscriptionNumber}",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Subscription Active</h2>
                <p>Your subscription <strong>{subscriptionNumber}</strong> has been activated.</p>
                <p>Your first delivery will be scheduled shortly.</p>
                </body></html>
                """,
            inAppTitle: "Subscription Active",
            inAppMessage: $"Your subscription #{subscriptionNumber} is now active.",
            type: NotificationType.SubscriptionCreated,
            referenceId: subscriptionId,
            referenceType: "Subscription");

    public Task SendDeliverySkippedAsync(User user, int deliveryId) =>
        TriggerAsync("delivery.skipped", user,
            inAppTitle: "Delivery Skipped",
            inAppMessage: "Your upcoming delivery has been skipped as requested.",
            type: NotificationType.DeliverySkipped,
            referenceId: deliveryId,
            referenceType: "Delivery");

    // ── Invoices ──────────────────────────────────────────────────────────────

    public Task SendInvoicePaidAsync(User user, string invoiceNumber, decimal total) =>
        TriggerAsync("invoice.paid", user,
            emailSubject: $"Payment Confirmed – Invoice {invoiceNumber}",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Payment Confirmed</h2>
                <p>Invoice <strong>{invoiceNumber}</strong> has been paid.</p>
                <p>Amount: <strong>₹{total:N2}</strong></p>
                </body></html>
                """,
            inAppTitle: "Invoice Paid",
            inAppMessage: $"Invoice #{invoiceNumber} (₹{total:N2}) has been paid.",
            type: NotificationType.InvoicePaid,
            referenceType: "Invoice");

    // ── Vendor ────────────────────────────────────────────────────────────────

    public Task SendVendorApprovedAsync(User user) =>
        TriggerAsync("vendor.approved", user,
            emailSubject: "Your Vendor Account is Approved!",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Vendor Account Approved!</h2>
                <p>Hi {user.FirstName}, your vendor account has been approved.</p>
                <p>You can now start listing products and accepting orders.</p>
                </body></html>
                """,
            inAppTitle: "Vendor Account Approved",
            inAppMessage: "Congratulations! Your vendor account has been approved. You can now list products.",
            type: NotificationType.VendorApproved);

    public Task SendVendorSuspendedAsync(User user) =>
        TriggerAsync("vendor.suspended", user,
            emailSubject: "Your Vendor Account Has Been Suspended",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Vendor Account Suspended</h2>
                <p>Hi {user.FirstName}, your vendor account has been suspended.</p>
                <p>Please contact our support team for more information.</p>
                </body></html>
                """,
            inAppTitle: "Vendor Account Suspended",
            inAppMessage: "Your vendor account has been suspended. Please contact support for details.",
            type: NotificationType.VendorSuspended);

    // ── Payouts ───────────────────────────────────────────────────────────────

    public Task SendPayoutProcessedAsync(User user, decimal netAmount) =>
        TriggerAsync("vendor.payout", user,
            emailSubject: "Payout Processed",
            emailBody: $"""
                <html><body style="font-family:Arial,sans-serif">
                <h2>Payout Processed</h2>
                <p>Hi {user.FirstName}, your payout of <strong>₹{netAmount:N2}</strong> has been processed.</p>
                <p>It will reflect in your account within 2–3 business days.</p>
                </body></html>
                """,
            inAppTitle: "Payout Processed",
            inAppMessage: $"Your payout of ₹{netAmount:N2} has been processed.",
            type: NotificationType.PayoutProcessed);

    // ── Helper ────────────────────────────────────────────────────────────────

    private async Task TriggerAsync(
        string eventKey,
        User user,
        string? emailSubject = null,
        string? emailBody = null,
        string? inAppTitle = null,
        string? inAppMessage = null,
        NotificationType? type = null,
        int? referenceId = null,
        string? referenceType = null)
    {
        var channels = new Dictionary<string, NotificationPayload>();

        if (emailSubject is not null && emailBody is not null)
            channels["email"] = new() { To = user.Email ?? string.Empty, Subject = emailSubject, Body = emailBody };

        if (inAppTitle is not null && inAppMessage is not null && type is not null)
        {
            var meta = new Dictionary<string, object> { ["type"] = type.Value.ToString() };
            if (referenceId is not null)   meta["referenceId"]   = referenceId;
            if (referenceType is not null) meta["referenceType"] = referenceType;

            channels["inapp"] = new()
            {
                To       = user.Id.ToString(),
                Subject  = inAppTitle,
                Body     = inAppMessage,
                Metadata = meta
            };
        }

        if (channels.Count == 0) return;

        var result = await _notify.TriggerAsync(eventKey, new NotifyContext
        {
            User = new NotifyUser
            {
                UserId        = user.Id.ToString(),
                Email         = user.Email,
                Phone         = user.PhoneNumber,
                PhoneVerified = user.IsPhoneVerified
            },
            Channels = channels
        });

        _logger.LogInformation(
            "Triggered notification [{EventKey}] for user {UserId}. AllSucceeded: {AllSucceeded}",
            eventKey, user.Id, result.AllSucceeded);

        if (!result.AllSucceeded)
            _logger.LogWarning(
                "Notification [{EventKey}] for user {UserId} had failures: {Failures}",
                eventKey, user.Id, string.Join(", ", result.Failures.Select(f => f.Error)));
    }
}
