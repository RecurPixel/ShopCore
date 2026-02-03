namespace ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;
    private readonly IDateTime _dateTime;

    public HandlePaymentWebhookCommandHandler(
        IApplicationDbContext context,
        IPaymentService paymentService,
        IDateTime dateTime)
    {
        _context = context;
        _paymentService = paymentService;
        _dateTime = dateTime;
    }

    public async Task Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify webhook signature
        if (!_paymentService.VerifyWebhookSignature(request.Payload, request.Signature))
            throw new ValidationException("Invalid webhook signature");

        // 2. Parse webhook payload
        var webhookEvent = _paymentService.ParseWebhookPayload(request.Payload);

        // 3. Handle different event types
        switch (webhookEvent.EventType)
        {
            case "payment.captured":
                await HandlePaymentCapturedAsync(webhookEvent, cancellationToken);
                break;

            case "payment.failed":
                await HandlePaymentFailedAsync(webhookEvent, cancellationToken);
                break;

            case "refund.processed":
                await HandleRefundProcessedAsync(webhookEvent, cancellationToken);
                break;

            // Additional events can be handled here
            default:
                // Log unknown event type but don't fail (idempotency)
                break;
        }
    }

    private async Task HandlePaymentCapturedAsync(WebhookEvent webhookEvent, CancellationToken ct)
    {
        // Try to find order first
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.RazorpayOrderId, ct);

        if (order != null)
        {
            // Idempotency check
            if (order.PaymentStatus == PaymentStatus.Paid)
                return;

            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidAt = _dateTime.UtcNow;
            order.PaymentTransactionId = webhookEvent.PaymentId;

            if (order.Status == OrderStatus.Pending)
            {
                order.SetInitialStatus(OrderStatus.Confirmed);

                foreach (var item in order.Items.Where(i => i.Status == OrderItemStatus.Pending))
                    item.Status = OrderItemStatus.Confirmed;

                _context.OrderStatusHistory.Add(new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = OrderStatus.Confirmed,
                    ChangedAt = _dateTime.UtcNow,
                    Notes = "Payment confirmed via webhook"
                });
            }

            await _context.SaveChangesAsync(ct);
            return;
        }

        // Try to find invoice
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == webhookEvent.RazorpayOrderId, ct);

        if (invoice != null)
        {
            // Idempotency check
            if (invoice.Status == InvoiceStatus.Paid)
                return;

            var amountPaid = invoice.Total - invoice.PaidAmount;
            invoice.PaidAmount = invoice.Total;
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = _dateTime.UtcNow;
            invoice.PaymentMethod = webhookEvent.Method ?? PaymentMethod.Online.ToString();
            invoice.PaymentTransactionId = webhookEvent.PaymentId;

            foreach (var delivery in invoice.Deliveries)
            {
                delivery.PaymentStatus = PaymentStatus.Paid;
                delivery.PaymentMethod = webhookEvent.Method ?? PaymentMethod.Online.ToString();
                delivery.PaymentTransactionId = webhookEvent.PaymentId;
                delivery.PaidAt = _dateTime.UtcNow;
            }

            invoice.Subscription.UnpaidAmount -= amountPaid;
            if (invoice.Subscription.UnpaidAmount < 0)
                invoice.Subscription.UnpaidAmount = 0;

            if (invoice.Subscription.Status == SubscriptionStatus.Suspended &&
                invoice.Subscription.UnpaidAmount < invoice.Subscription.CreditLimit)
            {
                invoice.Subscription.Status = SubscriptionStatus.Active;
            }

            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task HandlePaymentFailedAsync(WebhookEvent webhookEvent, CancellationToken ct)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.RazorpayOrderId, ct);

        if (order != null)
        {
            order.PaymentStatus = PaymentStatus.Failed;

            _context.OrderStatusHistory.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = order.Status,
                ChangedAt = _dateTime.UtcNow,
                Notes = "Payment failed"
            });

            await _context.SaveChangesAsync(ct);
            return;
        }

        // For invoices, just clear the transaction ID so they can try again
        var invoice = await _context.SubscriptionInvoices
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == webhookEvent.RazorpayOrderId, ct);

        if (invoice != null)
        {
            invoice.PaymentTransactionId = null;
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task HandleRefundProcessedAsync(WebhookEvent webhookEvent, CancellationToken ct)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.PaymentId, ct);

        if (order == null)
            return;

        order.RefundedAmount += webhookEvent.RefundAmount;

        if (order.RefundedAmount >= order.Total)
            order.PaymentStatus = PaymentStatus.Refunded;
        else
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;

        _context.OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = order.Status,
            ChangedAt = _dateTime.UtcNow,
            Notes = $"Refund of ₹{webhookEvent.RefundAmount} processed"
        });

        await _context.SaveChangesAsync(ct);
    }
}
