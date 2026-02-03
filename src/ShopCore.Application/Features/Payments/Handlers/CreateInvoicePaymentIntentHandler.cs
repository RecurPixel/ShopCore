using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

public class CreateInvoicePaymentIntentHandler
    : IRequestHandler<CreateInvoicePaymentIntentCommand, PaymentIntentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;

    public CreateInvoicePaymentIntentHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentService paymentService)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentService = paymentService;
    }

    public async Task<PaymentIntentDto> Handle(
        CreateInvoicePaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.User)
            .Include(i => i.Subscription)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice == null)
            throw new NotFoundException("Invoice", request.InvoiceId);

        // Verify ownership
        if (invoice.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only pay your own invoices");

        // Verify invoice is payable
        if (invoice.Status == InvoiceStatus.Paid)
            throw new ValidationException("Invoice is already paid");

        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new ValidationException("Cannot pay cancelled invoice");

        // Calculate amount to pay (balance due)
        var amountToCharge = invoice.BalanceDue;
        if (amountToCharge <= 0)
            throw new ValidationException("Nothing to pay");

        // Create payment intent via payment gateway (Razorpay)
        var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
            amountToCharge,
            invoice.Id,
            PaymentReferenceType.Invoice,
            "INR");

        // Store the payment reference
        invoice.PaymentTransactionId = paymentIntent.RazorpayOrderId;

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentIntent.RazorpayOrderId,
            ClientSecret = paymentIntent.KeyId,
            Amount = amountToCharge,
            Currency = paymentIntent.Currency,
            Status = PaymentStatus.Pending,
            RazorpayOrderId = paymentIntent.RazorpayOrderId
        };
    }
}