using ShopCore.Application.Orders.Commands.ProcessRefund;
using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Handlers;

public class ProcessRefundCommandHandler
    : IRequestHandler<ProcessRefundCommand, RefundResultDto>
{
    private readonly IApplicationDbContext _context;

    public ProcessRefundCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefundResultDto> Handle(
        ProcessRefundCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var itemsToRefund = order.Items
            .Where(i => request.OrderItemIds.Contains(i.Id))
            .ToList();

        if (itemsToRefund.Count != request.OrderItemIds.Count)
        {
            throw new NotFoundException("Some order items were not found");
        }

        decimal refundAmount = 0;

        foreach (var item in itemsToRefund)
        {
            if (item.Status == OrderItemStatus.Refunded)
            {
                continue; // Already refunded
            }

            item.Status = OrderItemStatus.Refunded;
            refundAmount += item.Subtotal;
        }

        order.RefundedAmount += refundAmount;
        order.UpdateStatusFromItems();

        // Update payment status
        if (order.RefundedAmount >= order.Total)
        {
            order.PaymentStatus = PaymentStatus.Refunded;
        }
        else if (order.RefundedAmount > 0)
        {
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Process actual refund via payment gateway

        return new RefundResultDto(
            order.Id,
            refundAmount,
            null, // RefundTransactionId - to be set after payment gateway call
            DateTime.UtcNow,
            "Processed"
        );
    }
}
