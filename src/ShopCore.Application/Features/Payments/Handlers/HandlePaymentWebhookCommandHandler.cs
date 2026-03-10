namespace ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

public class HandlePaymentWebhookCommandHandler : IRequestHandler<HandlePaymentWebhookCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IDateTime _dateTime;

    public HandlePaymentWebhookCommandHandler(
        IApplicationDbContext context,
        IPaymentGatewayFactory gatewayFactory,
        IDateTime dateTime)
    {
        _context = context;
        _gatewayFactory = gatewayFactory;
        _dateTime = dateTime;
    }

    public async Task Handle(HandlePaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the payment gateway
        var gateway = _gatewayFactory.GetGateway(request.Gateway);

        // 2. Verify webhook signature
        if (!string.IsNullOrEmpty(request.Signature) &&
            !gateway.VerifyWebhookSignature(request.Payload, request.Signature))
        {
            throw new ValidationException("Invalid webhook signature");
        }

        // 3. Parse webhook payload
        var webhookEvent = gateway.ParseWebhookPayload(request.Payload);

        // 4. Handle different event types
        switch (webhookEvent.EventType)
        {
            case PaymentWebhookEventType.PaymentCaptured:
                await HandlePaymentCapturedAsync(webhookEvent, cancellationToken);
                break;

            case PaymentWebhookEventType.PaymentFailed:
                await HandlePaymentFailedAsync(webhookEvent, cancellationToken);
                break;

            case PaymentWebhookEventType.RefundProcessed:
                await HandleRefundProcessedAsync(webhookEvent, cancellationToken);
                break;

            // Additional events can be handled here
            default:
                // Log unknown event type but don't fail (idempotency)
                break;
        }
    }

    private async Task HandlePaymentCapturedAsync(PaymentWebhookEvent webhookEvent, CancellationToken ct)
    {
        // Try to find order first
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.GatewayOrderId, ct);

        if (order != null)
        {
            // Idempotency check
            if (order.PaymentStatus == PaymentStatus.Paid)
                return;

            order.PaymentStatus = PaymentStatus.Paid;
            order.PaidAt = _dateTime.UtcNow;
            order.PaymentTransactionId = webhookEvent.GatewayPaymentId;

            if (webhookEvent.Method.HasValue)
                order.PaymentMethod = webhookEvent.Method.Value;

            if (order.Status == OrderStatus.Pending)
            {
                order.SetInitialStatus(OrderStatus.Confirmed);

                foreach (var item in order.Items.Where(i => i.Status == OrderItemStatus.Pending))
                    item.Status = OrderItemStatus.Confirmed;

                _context.OrderStatusHistories.Add(new OrderStatusHistory
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
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == webhookEvent.GatewayOrderId, ct);

        if (invoice != null)
        {
            // Idempotency check
            if (invoice.Status == InvoiceStatus.Paid)
                return;

            var amountPaid = invoice.Total - invoice.PaidAmount;
            invoice.PaidAmount = invoice.Total;
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = _dateTime.UtcNow;
            invoice.PaymentMethod = webhookEvent.Method ?? PaymentMethod.Online;
            invoice.PaymentTransactionId = webhookEvent.GatewayPaymentId;

            foreach (var delivery in invoice.Deliveries)
            {
                delivery.PaymentStatus = PaymentStatus.Paid;
                delivery.PaymentMethod = webhookEvent.Method ?? PaymentMethod.Online;
                delivery.PaymentTransactionId = webhookEvent.GatewayPaymentId;
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

    private async Task HandlePaymentFailedAsync(PaymentWebhookEvent webhookEvent, CancellationToken ct)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.GatewayOrderId, ct);

        if (order != null)
        {
            order.PaymentStatus = PaymentStatus.Failed;

            _context.OrderStatusHistories.Add(new OrderStatusHistory
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
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == webhookEvent.GatewayOrderId, ct);

        if (invoice != null)
        {
            invoice.PaymentTransactionId = null;
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task HandleRefundProcessedAsync(PaymentWebhookEvent webhookEvent, CancellationToken ct)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == webhookEvent.GatewayPaymentId, ct);

        if (order == null)
            return;

        order.RefundedAmount += webhookEvent.RefundAmount ?? 0;

        if (order.RefundedAmount >= order.Total)
            order.PaymentStatus = PaymentStatus.Refunded;
        else
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;

        _context.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = order.Status,
            ChangedAt = _dateTime.UtcNow,
            Notes = $"Refund of ₹{webhookEvent.RefundAmount} processed"
        });

        await _context.SaveChangesAsync(ct);
    }
}
