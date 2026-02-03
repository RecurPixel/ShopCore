using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.InitiateRefund;

public class InitiateRefundCommandHandler : IRequestHandler<InitiateRefundCommand, RefundDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;
    private readonly IDateTime _dateTime;

    public InitiateRefundCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentService paymentService,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentService = paymentService;
        _dateTime = dateTime;
    }

    public async Task<RefundDto> Handle(InitiateRefundCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        // Verify ownership or admin
        var isAdmin = _currentUser.Role == UserRole.Admin;
        if (order.UserId != _currentUser.UserId && !isAdmin)
            throw new ForbiddenException("Unauthorized to refund this order");

        // Verify order is paid
        if (order.PaymentStatus != PaymentStatus.Paid &&
            order.PaymentStatus != PaymentStatus.PartiallyRefunded)
            throw new ValidationException("Order is not paid");

        // Verify payment was online (not COD)
        if (order.PaymentMethod != PaymentMethod.Online)
            throw new ValidationException("Refund is only available for online payments");

        if (string.IsNullOrEmpty(order.PaymentTransactionId))
            throw new ValidationException("No payment transaction found for this order");

        // Calculate max refundable amount
        var maxRefundable = order.Total - order.RefundedAmount;
        if (request.Amount > maxRefundable)
            throw new ValidationException($"Maximum refundable amount is ₹{maxRefundable}");

        if (request.Amount <= 0)
            throw new ValidationException("Refund amount must be greater than zero");

        // Initiate refund with payment gateway
        var refundResponse = await _paymentService.CreateRefundAsync(
            order.PaymentTransactionId,
            request.Amount,
            request.Reason);

        // Update order refund tracking
        order.RefundedAmount += request.Amount;

        if (order.RefundedAmount >= order.Total)
            order.PaymentStatus = PaymentStatus.Refunded;
        else
            order.PaymentStatus = PaymentStatus.PartiallyRefunded;

        // Add status history
        _context.OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = order.Status,
            ChangedAt = _dateTime.UtcNow,
            Notes = $"Refund of ₹{request.Amount} initiated. Reason: {request.Reason ?? "Not specified"}"
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new RefundDto
        {
            RefundId = refundResponse.RefundId,
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            RefundAmount = request.Amount,
            Status = refundResponse.Status,
            RefundedAt = refundResponse.CreatedAt
        };
    }
}
