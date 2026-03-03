using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.PayInvoice;

public class PayInvoiceCommandHandler : IRequestHandler<PayInvoiceCommand, InvoiceDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    private readonly INotificationService _notificationService;

    public PayInvoiceCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime,
        INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _notificationService = notificationService;
    }

    public async Task<InvoiceDto> Handle(PayInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, ct);

        if (invoice == null)
            throw new NotFoundException("Invoice", request.InvoiceId);

        if (invoice.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only pay your own invoices");

        if (invoice.Status == InvoiceStatus.Paid)
            throw new ValidationException("Invoice is already paid");

        var amountToPay = invoice.Total - invoice.PaidAmount;

        invoice.PaidAmount = invoice.Total;
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = _dateTime.UtcNow;
        invoice.PaymentMethod = request.PaymentMethod;
        invoice.PaymentTransactionId = request.PaymentTransactionId;

        // Mark all linked deliveries as paid
        foreach (var delivery in invoice.Deliveries)
        {
            delivery.PaymentStatus = PaymentStatus.Paid;
            delivery.PaymentMethod = request.PaymentMethod;
            delivery.PaymentTransactionId = request.PaymentTransactionId;
            delivery.PaidAt = _dateTime.UtcNow;
        }

        // Reduce subscription unpaid amount
        invoice.Subscription.UnpaidAmount -= amountToPay;
        if (invoice.Subscription.UnpaidAmount < 0)
            invoice.Subscription.UnpaidAmount = 0;

        // Resume if suspended due to unpaid
        if (invoice.Subscription.Status == SubscriptionStatus.Suspended &&
            invoice.Subscription.UnpaidAmount < invoice.Subscription.CreditLimit)
        {
            invoice.Subscription.Status = SubscriptionStatus.Active;
        }

        await _context.SaveChangesAsync(ct);

        var user = await _context.Users.FindAsync(new object[] { invoice.UserId }, ct);
        if (user != null)
            await _notificationService.SendInvoicePaidAsync(user, invoice.InvoiceNumber, invoice.Total);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            Total = invoice.Total,
            Status = invoice.Status.ToString()
        };
    }
}
