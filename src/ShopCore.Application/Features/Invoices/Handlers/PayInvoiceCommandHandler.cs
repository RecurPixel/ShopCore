using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.PayInvoice;

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, InvoiceDto>
{
    public Task<InvoiceDto> Handle(PayInvoiceCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find invoice by id
        // 3. Verify user owns this invoice
        // 4. Check invoice is in pending/unpaid status
        // 5. Create payment intent for invoice amount
        // 6. Verify payment method
        // 7. Process payment through payment gateway
        // 8. Update invoice status to paid
        // 9. Create payment receipt and map return InvoiceDto
        return Task.FromResult(new InvoiceDto());
    }
}
