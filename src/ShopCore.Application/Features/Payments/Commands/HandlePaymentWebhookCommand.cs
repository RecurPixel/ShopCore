using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

/// <summary>
/// Command to handle a payment webhook from any gateway
/// </summary>
/// <param name="Gateway">The payment gateway sending the webhook</param>
/// <param name="Payload">The raw webhook payload</param>
/// <param name="Signature">The webhook signature for verification</param>
/// <param name="Headers">Optional headers that may contain signature info</param>
public record HandlePaymentWebhookCommand(
    PaymentGateway Gateway,
    string Payload,
    string? Signature,
    IDictionary<string, string>? Headers = null
) : IRequest;
