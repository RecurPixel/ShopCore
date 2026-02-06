namespace ShopCore.Application.Payments.Commands.RecordCODPayment;

public class RecordCODPaymentCommandHandler : IRequestHandler<RecordCODPaymentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public RecordCODPaymentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task Handle(RecordCODPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        // Verify payment method is COD
        if (order.PaymentMethod != PaymentMethod.CashOnDelivery)
            throw new ValidationException("Order payment method is not Cash on Delivery");

        // Verify order is in a state where COD can be recorded (Delivered or Out for Delivery)
        if (order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Shipped)
            throw new ValidationException("COD payment can only be recorded for delivered or shipped orders");

        // Verify not already paid
        if (order.PaymentStatus == PaymentStatus.Paid)
            throw new ValidationException("Order is already marked as paid");

        // Verify the current user is authorized (vendor who owns order items or admin)
        var isAdmin = _currentUser.Role == UserRole.Admin;
        var isVendorWithOrderItems = await _context.OrderItems
            .AnyAsync(oi => oi.OrderId == order.Id &&
                          oi.Product.VendorId == _currentUser.VendorId, cancellationToken);

        if (!isAdmin && !isVendorWithOrderItems)
            throw new ForbiddenException("Only the vendor or admin can record COD payment");

        // Mark as paid
        order.PaymentStatus = PaymentStatus.Paid;
        order.PaidAt = _dateTime.UtcNow;
        order.PaymentTransactionId = $"COD-{order.OrderNumber}-{_dateTime.UtcNow:yyyyMMddHHmmss}";

        // Add status history
        _context.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = order.Status,
            ChangedAt = _dateTime.UtcNow,
            Notes = $"COD payment of ₹{order.Total} collected"
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
