namespace ShopCore.Application.Invoices.Commands.PayInvoice;

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand>
{
    public Task Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
