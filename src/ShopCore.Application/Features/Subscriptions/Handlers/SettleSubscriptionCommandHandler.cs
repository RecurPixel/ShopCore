using ShopCore.Application.Common.Interfaces;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Subscriptions.Commands.SettleSubscription;

public class SettleSubscriptionCommandHandler
    : IRequestHandler<SettleSubscriptionCommand, SubscriptionSettlementDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentGatewayFactory _paymentGatewayFactory;
    private readonly IDateTime _dateTime;

    public SettleSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentGatewayFactory paymentGatewayFactory,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentGatewayFactory = paymentGatewayFactory;
        _dateTime = dateTime;
    }

    public async Task<SubscriptionSettlementDto> Handle(SettleSubscriptionCommand request, CancellationToken ct)
    {
        // 1. Find subscription with all deliveries and invoices
        var subscription = await _context.Subscriptions
            .Include(s => s.Deliveries)
            .Include(s => s.Invoices)
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.SubscriptionId);

        // 2. Verify ownership
        if (subscription.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only settle your own subscriptions");

        // 3. Calculate total delivered
        var totalDelivered = await _context.Deliveries
            .Where(d => d.SubscriptionId == subscription.Id && d.Status == DeliveryStatus.Delivered)
            .SumAsync(d => d.TotalAmount, ct);

        // 4. Calculate total paid
        var totalPaid = await _context.SubscriptionInvoices
            .Where(i => i.SubscriptionId == subscription.Id && i.Status == InvoiceStatus.Paid)
            .SumAsync(i => i.PaidAmount, ct);

        // 5. Calculate net balance (considering deposit)
        var depositBalance = subscription.DepositBalance ?? 0;
        var netBalance = totalDelivered - totalPaid - depositBalance;

        // 6. Handle settlement
        SubscriptionSettlementDto settlement;

        if (netBalance > 0)
        {
            // Customer owes vendor
            // Create payment intent for remaining amount
            var paymentGateway = _paymentGatewayFactory.GetGateway(PaymentGateway.Razorpay);
            var paymentIntent = await paymentGateway.CreatePaymentAsync(
                new CreatePaymentRequest
                {
                    Amount = netBalance,
                    Currency = "INR",
                    ReferenceId = subscription.Id,
                    ReferenceType = PaymentReferenceType.Invoice,
                    Description = $"Subscription Settlement {subscription.Id}"
                });

            settlement = new SubscriptionSettlementDto
            {
                SubscriptionId = subscription.Id,
                TotalDelivered = totalDelivered,
                TotalPaid = totalPaid,
                DepositUsed = depositBalance,
                NetBalance = netBalance,
                SettlementType = "CustomerOwes",
                PaymentRequired = true,
                PaymentIntentId = paymentIntent.GatewayOrderId,
                PaymentAmount = netBalance
            };
        }
        else if (netBalance < 0)
        {
            // Vendor owes customer (refund)
            var refundAmount = Math.Abs(netBalance);

            // Initiate refund if customer paid via online payment
            // (For now, just record the refund amount)
            settlement = new SubscriptionSettlementDto
            {
                SubscriptionId = subscription.Id,
                TotalDelivered = totalDelivered,
                TotalPaid = totalPaid,
                DepositUsed = depositBalance - refundAmount,
                NetBalance = netBalance,
                SettlementType = "VendorOwes",
                PaymentRequired = false,
                RefundAmount = refundAmount
            };
        }
        else
        {
            // All settled
            subscription.Status = SubscriptionStatus.Settled;
            subscription.CancelledAt = _dateTime.UtcNow;
            subscription.DepositBalance = 0;

            await _context.SaveChangesAsync(ct);

            settlement = new SubscriptionSettlementDto
            {
                SubscriptionId = subscription.Id,
                TotalDelivered = totalDelivered,
                TotalPaid = totalPaid,
                DepositUsed = depositBalance,
                NetBalance = 0,
                SettlementType = "Settled",
                PaymentRequired = false
            };
        }

        return settlement;
    }
}
