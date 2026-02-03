using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;

public class UpdateVendorOrderStatusCommandHandler
    : IRequestHandler<UpdateVendorOrderStatusCommand, VendorOrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateVendorOrderStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorOrderDto> Handle(
        UpdateVendorOrderStatusCommand request,
        CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        // Verify vendor owns items in this order
        var vendorItems = order.Items
            .Where(i => i.VendorId == _currentUser.VendorId)
            .ToList();

        if (!vendorItems.Any())
            throw new ForbiddenException("You don't have any items in this order");

        // Validate status transition
        ValidateStatusTransition(order.Status, request.Status);

        order.Status = request.Status;
        order.Notes = request.Notes;

        await _context.SaveChangesAsync(ct);

        return new VendorOrderDto(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.User.FullName,
            order.Status,
            order.Subtotal,
            order.Total,
            order.CreatedAt,
            order.Notes,
            vendorItems.Select(i => new VendorOrderItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.ImageUrl,
                i.Quantity,
                i.UnitPrice,
                i.Subtotal
            )).ToList()
        );
    }

    private static void ValidateStatusTransition(OrderStatus current, OrderStatus next)
    {
        var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
        {
            { OrderStatus.Pending, new[] { OrderStatus.Confirmed, OrderStatus.Cancelled } },
            { OrderStatus.Confirmed, new[] { OrderStatus.Processing, OrderStatus.Cancelled } },
            { OrderStatus.Processing, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
            { OrderStatus.Shipped, new[] { OrderStatus.Delivered } },
            { OrderStatus.Delivered, Array.Empty<OrderStatus>() },
            { OrderStatus.Cancelled, Array.Empty<OrderStatus>() }
        };

        if (!validTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(next))
            throw new BadRequestException($"Cannot transition from {current} to {next}");
    }
}
