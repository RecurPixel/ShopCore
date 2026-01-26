using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;

public class GenerateSubscriptionInvoiceCommandHandler
    : IRequestHandler<GenerateSubscriptionInvoiceCommand, InvoiceDto>
{
    public Task<InvoiceDto> Handle(
        GenerateSubscriptionInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new InvoiceDto());
    }
}
