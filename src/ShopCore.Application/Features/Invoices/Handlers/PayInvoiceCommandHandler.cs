using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.PayInvoice;

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, InvoiceDto>
{
    public Task<InvoiceDto> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new InvoiceDto());
    }
}
