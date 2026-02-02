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
        var refundResult = await _paymentService.ProcessRefundAsync(
            order.PaymentTransactionId!,
            refundAmount,
            request.Reason);

        if (!refundResult.IsSuccess)
            throw new ValidationException($"Refund failed: {refundResult.ErrorMessage}");

        // Update order
        order.RefundedAmount = (order.RefundedAmount ?? 0) + refundAmount;
        order.PaymentStatus = order.RefundedAmount >= order.Total
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        // Update items
        foreach (var item in itemsToRefund)
        {
            item.Status = OrderStatus.Refunded;
        }

        // Update order status
        if (order.Items.All(i => i.Status == OrderStatus.Refunded))
            order.Status = OrderStatus.Refunded;
        else if (order.Items.Any(i => i.Status == OrderStatus.Refunded))
            order.Status = OrderStatus.PartiallyRefunded;

        await _context.SaveChangesAsync(ct);

        return new RefundResultDto
        {
            IsSuccess = true,
            RefundAmount = refundAmount,
            TransactionId = refundResult.TransactionId
        };
    }
}
