using ShopCore.Application.Common.Models;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler
    : IRequestHandler<ConfirmPaymentCommand, PaymentConfirmationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly IDateTime _dateTime;

    public ConfirmPaymentCommandHandler(
        IApplicationDbContext context,
        IPaymentGatewayFactory gatewayFactory,
        IDateTime dateTime)
    {
        _context = context;
        _gatewayFactory = gatewayFactory;
        _dateTime = dateTime;
    }

    public async Task<PaymentConfirmationDto> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get the payment gateway
        var gateway = _gatewayFactory.GetGateway(request.Gateway);

        // 2. Verify payment with the gateway
        var verifyResult = await gateway.VerifyPaymentAsync(new VerifyPaymentRequest
        {
            GatewayOrderId = request.GatewayOrderId,
            GatewayPaymentId = request.GatewayPaymentId,
            Signature = request.Signature,
            AdditionalData = request.AdditionalData
        }, cancellationToken);

        if (!verifyResult.IsValid)
            throw new ValidationException(verifyResult.ErrorMessage ?? "Payment verification failed");

        // 3. Try to find the order by PaymentTransactionId
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.PaymentTransactionId == request.GatewayOrderId, cancellationToken);

        if (order != null)
        {
            return await ConfirmOrderPaymentAsync(order, request.GatewayPaymentId, verifyResult.Method, cancellationToken);
        }

        // 4. Try to find the invoice by PaymentTransactionId
        var invoice = await _context.SubscriptionInvoices
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
            .FirstOrDefaultAsync(i => i.PaymentTransactionId == request.GatewayOrderId, cancellationToken);

        if (invoice != null)
        {
            return await ConfirmInvoicePaymentAsync(invoice, request.GatewayPaymentId, verifyResult.Method, cancellationToken);
        }

        throw new NotFoundException("Payment reference not found for the given gateway order ID");
    }

    private async Task<PaymentConfirmationDto> ConfirmOrderPaymentAsync(
        Order order,
        string gatewayPaymentId,
        PaymentMethod? method,
        CancellationToken cancellationToken)
    {
        // Update order payment status
        order.PaymentStatus = PaymentStatus.Paid;
        order.PaidAt = _dateTime.UtcNow;
        order.PaymentTransactionId = gatewayPaymentId;

        if (method.HasValue)
            order.PaymentMethod = method.Value;

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
            _context.OrderStatusHistories.Add(new OrderStatusHistory
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
            PaymentId = gatewayPaymentId,
            Status = PaymentStatus.Paid.ToString(),
            Amount = order.Total,
            ConfirmedAt = order.PaidAt
        };
    }

    private async Task<PaymentConfirmationDto> ConfirmInvoicePaymentAsync(
        SubscriptionInvoice invoice,
        string gatewayPaymentId,
        PaymentMethod? method,
        CancellationToken cancellationToken)
    {
        var amountPaid = invoice.Total - invoice.PaidAmount;

        // Update invoice payment status
        invoice.PaidAmount = invoice.Total;
        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = _dateTime.UtcNow;
        invoice.PaymentMethod = method ?? PaymentMethod.Online;
        invoice.PaymentTransactionId = gatewayPaymentId;

        // Mark all linked deliveries as paid
        foreach (var delivery in invoice.Deliveries)
        {
            delivery.PaymentStatus = PaymentStatus.Paid;
            delivery.PaymentMethod = method ?? PaymentMethod.Online;
            delivery.PaymentTransactionId = gatewayPaymentId;
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
            PaymentId = gatewayPaymentId,
            Status = PaymentStatus.Paid.ToString(),
            Amount = invoice.Total,
            ConfirmedAt = invoice.PaidAt
        };
    }
}
