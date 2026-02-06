using ShopCore.Application.Orders.Commands.ProcessRefund;
using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Handlers;

public class ProcessRefundCommandHandler
    : IRequestHandler<ProcessRefundCommand, RefundResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;

    public ProcessRefundCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentService paymentService)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentService = paymentService;
    }

    public async Task<RefundResultDto> Handle(ProcessRefundCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can process refunds");

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.PaymentStatus != PaymentStatus.Paid)
            throw new ValidationException("Order must be paid to process refund");

        // Calculate refund amount from specific items
        var itemsToRefund = order.Items.Where(i => request.OrderItemIds.Contains(i.Id)).ToList();
        if (!itemsToRefund.Any())
            throw new ValidationException("No valid items to refund");

        var refundAmount = itemsToRefund.Sum(i => i.Subtotal);

        // Process refund via payment gateway
        var refundResult = await _paymentService.CreateRefundAsync(
            order.PaymentTransactionId!,
            refundAmount,
            request.Reason);

        if (refundResult.Status.Equals("failed", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("Refund failed");

        // Update order
        order.RefundedAmount += refundAmount;
        order.PaymentStatus = order.RefundedAmount >= order.Total
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        // Update items
        foreach (var item in itemsToRefund)
        {
            item.Status = OrderItemStatus.Refunded;
        }

        // Update order status based on items (derived status)
        order.UpdateStatusFromItems();

        await _context.SaveChangesAsync(ct);

        return new RefundResultDto
        {
            OrderId = order.Id,
            RefundAmount = refundAmount,
            RefundTransactionId = refundResult.RefundId,
            RefundedAt = DateTime.UtcNow,
            Status = "Initiated"
        };
    }
}
