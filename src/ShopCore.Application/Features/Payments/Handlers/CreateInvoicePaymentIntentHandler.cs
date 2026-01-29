using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

public class CreateInvoicePaymentIntentHandler
    : IRequestHandler<CreateInvoicePaymentIntentCommand, PaymentIntentDto>
{

    private readonly IApplicationDbContext _context;

    public CreateInvoicePaymentIntentHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentIntentDto> Handle(
        CreateInvoicePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        // var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);
        // if (invoice == null)
        //     throw new NotFoundException("Invoice not found");

        // // Business logic for invoice payment (subscription)
        // var amount = invoice.TotalAmount;
        // var currency = invoice.Currency;

        // return await _paymentService.CreatePaymentIntentAsync(
        //     amount,
        //     currency,
        //     PaymentIntentType.Invoice,
        //     invoice.Id,
        //     invoice.CustomerId
        // );

        return Task.FromResult(new PaymentIntentDto());
    }
}