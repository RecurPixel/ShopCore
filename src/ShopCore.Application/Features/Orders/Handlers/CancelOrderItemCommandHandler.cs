using ShopCore.Application.Orders.Commands.CancelOrderItem;
using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Handlers;

public class CancelOrderItemCommandHandler
    : IRequestHandler<CancelOrderItemCommand, CancellationResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderItemCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CancellationResultDto> Handle(
        CancelOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId, cancellationToken)
            ?? throw new NotFoundException(nameof(OrderItem), request.OrderItemId);

        // Check if user owns the order or is the vendor
        var order = orderItem.Order;
        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken);

        var isOwner = order.UserId == userId;
        var isVendor = vendor != null && orderItem.VendorId == vendor.Id;

        if (!isOwner && !isVendor)
        {
            throw new UnauthorizedAccessException("You don't have permission to cancel this item");
        }

        // Can only cancel before shipping
        if (orderItem.Status == OrderItemStatus.Shipped ||
            orderItem.Status == OrderItemStatus.Delivered ||
            orderItem.Status == OrderItemStatus.Cancelled ||
            orderItem.Status == OrderItemStatus.Refunded)
        {
            throw new InvalidOperationException($"Cannot cancel item with status {orderItem.Status}");
        }

        orderItem.Status = OrderItemStatus.Cancelled;
        orderItem.CancellationReason = request.Reason;

        // Calculate refund amount
        var refundAmount = orderItem.Subtotal;

        // Update parent order
        var fullOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        if (fullOrder != null)
        {
            fullOrder.UpdateStatusFromItems();
            fullOrder.RefundedAmount += refundAmount;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CancellationResultDto(
            order.Id,
            orderItem.Id,
            DateTime.UtcNow,
            refundAmount,
            "Pending" // TODO: Integrate with payment gateway
        );
    }
}
