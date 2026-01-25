namespace ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

public record HandlePaymentWebhookCommand(string Payload, string Signature) : IRequest;
