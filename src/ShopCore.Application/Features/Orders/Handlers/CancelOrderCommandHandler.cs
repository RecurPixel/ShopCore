using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;
    private readonly INotificationService _notificationService;

    public CancelOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime,
        INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _notificationService = notificationService;
    }

    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.UserId != _currentUser.UserId && _currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("You can only cancel your own orders");

        // Cannot cancel if already shipped or delivered
        if (order.Status == OrderStatus.Shipped ||
            order.Status == OrderStatus.Delivered ||
            order.Status == OrderStatus.Cancelled)
            throw new ValidationException($"Cannot cancel order in {order.Status} status");

        // Cancel all items
        foreach (var item in order.Items)
        {
            if (item.Status != OrderItemStatus.Shipped && item.Status != OrderItemStatus.Delivered)
            {
                item.Status = OrderItemStatus.Cancelled;
                item.CancellationReason = request.Reason;

                // Restore stock
                if (item.Product.TrackInventory)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
            }
        }

        // Update order status based on items (derived status)
        order.UpdateStatusFromItems();
        order.CancelledAt = _dateTime.UtcNow;
        order.CancellationReason = request.Reason;

        // Add status history
        _context.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Cancelled,
            Notes = request.Reason,
            ChangedAt = _dateTime.UtcNow,
            ChangedBy = _currentUser.UserId
        });

        await _context.SaveChangesAsync(ct);

        // Notify order owner
        var user = await _context.Users.FindAsync(new object[] { order.UserId }, ct);
        if (user != null)
            await _notificationService.SendOrderCancelledAsync(user, order.Id, order.OrderNumber, request.Reason);

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString()
        };
    }
}
