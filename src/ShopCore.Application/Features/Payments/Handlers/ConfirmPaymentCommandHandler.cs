using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler
    : IRequestHandler<ConfirmPaymentCommand, PaymentConfirmationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IDateTime _dateTime;

    public ConfirmPaymentCommandHandler(
        IApplicationDbContext context,
        IPaymentService paymentService,
        IDateTime dateTime)
    {
        _context = context;
        _paymentService = paymentService;
        _dateTime = dateTime;
    }

    public async Task<PaymentConfirmationDto> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verify payment signature with Razorpay
        var isValid = await _paymentService.VerifyPaymentSignatureAsync(
            request.RazorpayOrderId,
            request.RazorpayPaymentId,
            request.RazorpaySignature);

        if (!isValid)
            throw new ValidationException("Payment signature verification failed");

        // 2. Try to find the order by PaymentTransactionId
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == request.RazorpayOrderId, cancellationToken);

        if (order != null)
        {
            return await ConfirmOrderPaymentAsync(order, request.RazorpayPaymentId, cancellationToken);
        }

        // 3. Try to find the invoice by PaymentTransactionId
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == request.RazorpayOrderId, cancellationToken);

        if (invoice != null)
        {
            return await ConfirmInvoicePaymentAsync(invoice, request.RazorpayPaymentId, cancellationToken);
        }

        throw new NotFoundException("Payment reference not found for the given Razorpay order ID");
    }

    private async Task<PaymentConfirmationDto> ConfirmOrderPaymentAsync(
        Order order,
        string razorpayPaymentId,
        CancellationToken cancellationToken)
    {
        // Update order payment status
        order.PaymentStatus = PaymentStatus.Paid;
        order.PaidAt = _dateTime.UtcNow;
        order.PaymentTransactionId = razorpayPaymentId;

        // Update order status to Confirmed if pending
        if (order.Status == OrderStatus.Pending)
        {
            order.SetInitialStatus(OrderStatus.Confirmed);

            // Update all order items to Confirmed
            foreach (var item in order.Items)
            {
                if (item.Status == OrderItemStatus.Pending)
                    item.Status = OrderItemStatus.Confirmed;
            }

            // Add status history
            _context.OrderStatusHistory.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Confirmed,
                ChangedAt = _dateTime.UtcNow,
                Notes = "Payment confirmed"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentConfirmationDto
        {
            ReferenceId = order.Id,
            ReferenceType = "Order",
            ReferenceNumber = order.OrderNumber,
            PaymentId = razorpayPaymentId,
            Status = PaymentStatus.Paid,
            Amount = order.Total,
            ConfirmedAt = order.PaidAt
        };
    }

    private async Task<PaymentConfirmationDto> ConfirmInvoicePaymentAsync(
        SubscriptionInvoice invoice,
        string razorpayPaymentId,
        CancellationToken cancellationToken)
    {
        var amountPaid = invoice.Total - invoice.PaidAmount;

        // Update invoice payment status
        invoice.PaidAmount = invoice.Total;
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = _dateTime.UtcNow;
        invoice.PaymentMethod = PaymentMethod.Online;
        invoice.PaymentTransactionId = razorpayPaymentId;

        // Mark all linked deliveries as paid
        foreach (var delivery in invoice.Deliveries)
        {
            delivery.PaymentStatus = PaymentStatus.Paid;
            delivery.PaymentMethod = PaymentMethod.Online;
            delivery.PaymentTransactionId = razorpayPaymentId;
            delivery.PaidAt = _dateTime.UtcNow;
        }

        // Update subscription unpaid amount
        invoice.Subscription.UnpaidAmount -= amountPaid;
        if (invoice.Subscription.UnpaidAmount < 0)
            invoice.Subscription.UnpaidAmount = 0;

        // Resume subscription if suspended due to unpaid balance
        if (invoice.Subscription.Status == SubscriptionStatus.Suspended &&
            invoice.Subscription.UnpaidAmount < invoice.Subscription.CreditLimit)
        {
            invoice.Subscription.Status = SubscriptionStatus.Active;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentConfirmationDto
        {
            ReferenceId = invoice.Id,
            ReferenceType = "Invoice",
            ReferenceNumber = invoice.InvoiceNumber,
            PaymentId = razorpayPaymentId,
            Status = PaymentStatus.Paid,
            Amount = invoice.Total,
            ConfirmedAt = invoice.PaidAt
        };
    }
}
