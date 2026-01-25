namespace ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand>
{
    public Task Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
