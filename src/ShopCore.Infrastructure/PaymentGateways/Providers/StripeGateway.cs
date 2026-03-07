using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;
using Stripe;
using DomainPaymentMethod = ShopCore.Domain.Enums.PaymentMethod;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Stripe payment gateway implementation using official SDK
/// </summary>
public class StripeGateway : BasePaymentGateway
{
    private readonly StripeOptions _options;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;

    public StripeGateway(IOptions<PaymentGatewayOptions> options)
    {
        _options = options.Value.Stripe;

        // Configure Stripe API key
        StripeConfiguration.ApiKey = _options.SecretKey;

        _paymentIntentService = new PaymentIntentService();
        _refundService = new RefundService();
    }

    public override PaymentGateway GatewayType => PaymentGateway.Stripe;
    public override string DisplayName => "Stripe";
    public override string Description => "Pay securely with Card, Apple Pay, Google Pay, or other methods";

    public override IReadOnlyCollection<DomainPaymentMethod> SupportedMethods =>
        [DomainPaymentMethod.Card, DomainPaymentMethod.Wallet];

    public override async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default)
    {
        try
        {
            long amountInCents = (long)(request.Amount * 100);

            PaymentIntentCreateOptions createOptions = new()
            {
                Amount = amountInCents,
                Currency = request.Currency.ToLower(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["reference_id"] = request.ReferenceId.ToString(),
                    ["reference_type"] = request.ReferenceType.ToString()
                },
                Description = request.Description ?? $"Payment for {request.ReferenceType} #{request.ReferenceId}"
            };

            // Add customer email if provided
            if (!string.IsNullOrEmpty(request.CustomerEmail))
            {
                createOptions.ReceiptEmail = request.CustomerEmail;
            }

            Stripe.PaymentIntent paymentIntent = await _paymentIntentService.CreateAsync(createOptions, cancellationToken: ct);

            return new CreatePaymentResult
            {
                Success = true,
                GatewayOrderId = paymentIntent.Id,
                GatewayPaymentId = paymentIntent.Id,
                Amount = request.Amount,
                Currency = request.Currency.ToLower(),
                Gateway = PaymentGateway.Stripe,
                ClientData = new Dictionary<string, string>
                {
                    ["publishable_key"] = _options.PublishableKey,
                    ["client_secret"] = paymentIntent.ClientSecret,
                    ["payment_intent_id"] = paymentIntent.Id,
                    ["amount"] = amountInCents.ToString(),
                    ["currency"] = request.Currency.ToLower()
                },
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType
            };
        }
        catch (StripeException ex)
        {
            return new CreatePaymentResult
            {
                Success = false,
                ErrorMessage = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}",
                Gateway = PaymentGateway.Stripe,
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType
            };
        }
        catch (Exception ex)
        {
            return new CreatePaymentResult
            {
                Success = false,
                ErrorMessage = $"Failed to create payment intent: {ex.Message}",
                Gateway = PaymentGateway.Stripe,
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType
            };
        }
    }

    public override async Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default)
    {
        try
        {
            // Stripe uses payment intent status to verify payment
            Stripe.PaymentIntent paymentIntent = await _paymentIntentService.GetAsync(request.GatewayPaymentId, cancellationToken: ct);

            bool isValid;
            PaymentStatus status;

            switch (paymentIntent.Status)
            {
                case "succeeded":
                    isValid = true;
                    status = PaymentStatus.Paid;
                    break;
                case "processing":
                    isValid = true;
                    status = PaymentStatus.Pending;
                    break;
                case "requires_payment_method":
                case "requires_confirmation":
                case "requires_action":
                    isValid = false;
                    status = PaymentStatus.Pending;
                    break;
                case "canceled":
                    isValid = false;
                    status = PaymentStatus.Failed;
                    break;
                default:
                    isValid = false;
                    status = PaymentStatus.Failed;
                    break;
            }

            return new VerifyPaymentResult
            {
                IsValid = isValid,
                Status = status,
                GatewayPaymentId = paymentIntent.Id,
                Method = ParseStripePaymentMethod(paymentIntent.PaymentMethod?.Type),
                ErrorMessage = isValid ? null : $"Payment status: {paymentIntent.Status}"
            };
        }
        catch (StripeException ex)
        {
            return new VerifyPaymentResult
            {
                IsValid = false,
                Status = PaymentStatus.Failed,
                GatewayPaymentId = request.GatewayPaymentId,
                ErrorMessage = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new VerifyPaymentResult
            {
                IsValid = false,
                Status = PaymentStatus.Failed,
                GatewayPaymentId = request.GatewayPaymentId,
                ErrorMessage = $"Verification failed: {ex.Message}"
            };
        }
    }

    public override async Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default)
    {
        try
        {
            PaymentIntentGetOptions options = new();
            options.AddExpand("payment_method");

            Stripe.PaymentIntent paymentIntent = await _paymentIntentService.GetAsync(gatewayPaymentId, options, cancellationToken: ct);

            PaymentStatus status = paymentIntent.Status switch
            {
                "succeeded" => PaymentStatus.Paid,
                "processing" => PaymentStatus.Pending,
                "canceled" => PaymentStatus.Failed,
                "requires_payment_method" or "requires_confirmation" or "requires_action" => PaymentStatus.Pending,
                _ => PaymentStatus.Failed
            };

            return new PaymentStatusResult
            {
                Success = true,
                GatewayPaymentId = paymentIntent.Id,
                Status = status,
                Amount = paymentIntent.Amount / 100m,
                Method = ParseStripePaymentMethod(paymentIntent.PaymentMethod?.Type),
                CreatedAt = paymentIntent.Created,
                CompletedAt = status == PaymentStatus.Paid ? DateTime.UtcNow : null
            };
        }
        catch (StripeException ex)
        {
            return new PaymentStatusResult
            {
                Success = false,
                ErrorMessage = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}",
                GatewayPaymentId = gatewayPaymentId,
                Status = PaymentStatus.Pending
            };
        }
        catch (Exception ex)
        {
            return new PaymentStatusResult
            {
                Success = false,
                ErrorMessage = $"Failed to fetch payment status: {ex.Message}",
                GatewayPaymentId = gatewayPaymentId,
                Status = PaymentStatus.Pending
            };
        }
    }

    public override async Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default)
    {
        try
        {
            long amountInCents = (long)(request.Amount * 100);

            RefundCreateOptions refundOptions = new()
            {
                PaymentIntent = request.GatewayPaymentId,
                Amount = amountInCents,
                Reason = MapRefundReason(request.Reason)
            };

            if (request.Metadata?.Count > 0)
            {
                refundOptions.Metadata = request.Metadata.ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            Stripe.Refund refund = await _refundService.CreateAsync(refundOptions, cancellationToken: ct);

            RefundStatus refundStatus = refund.Status switch
            {
                "succeeded" => RefundStatus.Completed,
                "pending" => RefundStatus.Pending,
                "failed" => RefundStatus.Failed,
                "canceled" => RefundStatus.Failed,
                _ => RefundStatus.Processing
            };

            return new RefundResult
            {
                Success = refund.Status != "failed" && refund.Status != "canceled",
                RefundId = refund.Id,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = refund.Amount / 100m,
                Status = refundStatus,
                CreatedAt = refund.Created,
                ErrorMessage = refund.FailureReason
            };
        }
        catch (StripeException ex)
        {
            return new RefundResult
            {
                Success = false,
                ErrorMessage = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}",
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Status = RefundStatus.Failed,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new RefundResult
            {
                Success = false,
                ErrorMessage = $"Failed to create refund: {ex.Message}",
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Status = RefundStatus.Failed,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public override bool VerifyWebhookSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_options.WebhookSecret))
            return true;

        try
        {
            // Use Stripe SDK to verify webhook signature
            EventUtility.ConstructEvent(payload, signature, _options.WebhookSecret);
            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }

    public override PaymentWebhookEvent ParseWebhookPayload(string payload)
    {
        Event stripeEvent = EventUtility.ParseEvent(payload, throwOnApiVersionMismatch: false);
        PaymentWebhookEventType eventType = MapEventType(stripeEvent.Type);

        PaymentWebhookEvent webhookEvent = new()
        {
            EventType = eventType,
            Gateway = PaymentGateway.Stripe
        };

        // Extract data based on event type
        switch (stripeEvent.Data.Object)
        {
            case Stripe.PaymentIntent paymentIntent:
                webhookEvent = webhookEvent with
                {
                    GatewayPaymentId = paymentIntent.Id,
                    GatewayOrderId = paymentIntent.Id,
                    Amount = paymentIntent.Amount / 100m,
                    Method = ParseStripePaymentMethod(paymentIntent.PaymentMethod?.Type)
                };

                // Extract reference info from metadata
                if (paymentIntent.Metadata.TryGetValue("reference_id", out string? refId) &&
                    int.TryParse(refId, out int referenceId))
                {
                    webhookEvent = webhookEvent with { ReferenceId = referenceId };
                }

                if (paymentIntent.Metadata.TryGetValue("reference_type", out string? refType) &&
                    Enum.TryParse<PaymentReferenceType>(refType, out PaymentReferenceType referenceType))
                {
                    webhookEvent = webhookEvent with { ReferenceType = referenceType };
                }
                break;

            case Stripe.Refund refund:
                webhookEvent = webhookEvent with
                {
                    GatewayPaymentId = refund.PaymentIntentId,
                    RefundId = refund.Id,
                    RefundAmount = refund.Amount / 100m
                };
                break;

            case Charge charge:
                webhookEvent = webhookEvent with
                {
                    GatewayPaymentId = charge.PaymentIntentId ?? charge.Id,
                    Amount = charge.Amount / 100m,
                    Method = DomainPaymentMethod.Card
                };

                if (charge.Refunded)
                {
                    webhookEvent = webhookEvent with
                    {
                        RefundAmount = charge.AmountRefunded / 100m
                    };
                }
                break;
        }

        return webhookEvent;
    }

    private static DomainPaymentMethod ParseStripePaymentMethod(string? methodType)
    {
        return methodType switch
        {
            "card" => DomainPaymentMethod.Card,
            "card_present" => DomainPaymentMethod.Card,
            "apple_pay" or "google_pay" => DomainPaymentMethod.Wallet,
            "link" => DomainPaymentMethod.Wallet,
            _ => DomainPaymentMethod.Online
        };
    }

    private static string? MapRefundReason(string? reason)
    {
        if (string.IsNullOrEmpty(reason))
            return null;

        // Stripe only accepts specific refund reasons
        return reason.ToLower() switch
        {
            "duplicate" => "duplicate",
            "fraudulent" => "fraudulent",
            "requested_by_customer" or "customer_request" or "customer" => "requested_by_customer",
            _ => null // Use null for other reasons
        };
    }

    private static PaymentWebhookEventType MapEventType(string eventType)
    {
        return eventType switch
        {
            "payment_intent.succeeded" => PaymentWebhookEventType.PaymentCaptured,
            "payment_intent.payment_failed" => PaymentWebhookEventType.PaymentFailed,
            "payment_intent.created" => PaymentWebhookEventType.PaymentCreated,
            "payment_intent.canceled" => PaymentWebhookEventType.PaymentFailed,
            "charge.refunded" => PaymentWebhookEventType.RefundProcessed,
            "refund.created" => PaymentWebhookEventType.RefundCreated,
            "refund.updated" => PaymentWebhookEventType.RefundProcessed,
            "refund.failed" => PaymentWebhookEventType.RefundFailed,
            _ => PaymentWebhookEventType.Unknown
        };
    }
}
