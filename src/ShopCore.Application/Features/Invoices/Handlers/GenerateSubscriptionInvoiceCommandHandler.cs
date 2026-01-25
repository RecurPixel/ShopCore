namespace ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;

public class GenerateSubscriptionInvoiceCommandHandler
    : IRequestHandler<GenerateSubscriptionInvoiceCommand>
{
    public Task Handle(
        GenerateSubscriptionInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
