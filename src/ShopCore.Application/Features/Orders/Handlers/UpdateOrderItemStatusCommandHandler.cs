using ShopCore.Application.Orders.Commands.UpdateOrderItemStatus;

namespace ShopCore.Application.Orders.Handlers;

public class UpdateOrderItemStatusCommandHandler
    : IRequestHandler<UpdateOrderItemStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateOrderItemStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        UpdateOrderItemStatusCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        var orderItem = await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId && oi.VendorId == vendor.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(OrderItem), request.OrderItemId);

        // Validate status transition
        ValidateStatusTransition(orderItem.Status, request.NewStatus);

        orderItem.Status = request.NewStatus;

        if (request.NewStatus == OrderItemStatus.Shipped)
        {
            orderItem.TrackingNumber = request.TrackingNumber;
            orderItem.ShippedAt = DateTime.UtcNow;
        }
        else if (request.NewStatus == OrderItemStatus.Delivered)
        {
            orderItem.DeliveredAt = DateTime.UtcNow;
        }

        // Update parent order status
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderItem.OrderId, cancellationToken);

        if (order != null)
        {
            order.UpdateStatusFromItems();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateStatusTransition(OrderItemStatus currentStatus, OrderItemStatus newStatus)
    {
        var validTransitions = new Dictionary<OrderItemStatus, OrderItemStatus[]>
        {
            [OrderItemStatus.Pending] = [OrderItemStatus.Confirmed, OrderItemStatus.Cancelled],
            [OrderItemStatus.Confirmed] = [OrderItemStatus.Processing, OrderItemStatus.Cancelled],
            [OrderItemStatus.Processing] = [OrderItemStatus.Shipped, OrderItemStatus.Cancelled],
            [OrderItemStatus.Shipped] = [OrderItemStatus.Delivered],
            [OrderItemStatus.Delivered] = [OrderItemStatus.Refunded],
            [OrderItemStatus.Cancelled] = [],
            [OrderItemStatus.Refunded] = []
        };

        if (!validTransitions.TryGetValue(currentStatus, out var allowed) || !allowed.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {currentStatus} to {newStatus}");
        }
    }
}
