using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

public class CreateOrderPaymentIntentHandler
    : IRequestHandler<CreateOrderPaymentIntentCommand, PaymentIntentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentService _paymentService;

    public CreateOrderPaymentIntentHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentService paymentService)
    {
        _context = context;
        _currentUser = currentUser;
        _paymentService = paymentService;
    }

    public async Task<PaymentIntentDto> Handle(
        CreateOrderPaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        // Verify ownership
        if (order.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only pay for your own orders");

        // Verify order is payable
        if (order.PaymentStatus == PaymentStatus.Paid)
            throw new ValidationException("Order is already paid");

        if (order.Status == OrderStatus.Cancelled)
            throw new ValidationException("Cannot pay for cancelled order");

        // Calculate amount (handle partial refunds)
        var amountToCharge = order.Total - order.RefundedAmount;
        if (amountToCharge <= 0)
            throw new ValidationException("Nothing to pay");

        // Create payment intent via payment gateway (Razorpay)
        var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
            amountToCharge,
            order.Id,
            PaymentReferenceType.Order,
            "INR");

        // Update order payment status to pending
        order.PaymentStatus = PaymentStatus.Pending;
        order.PaymentTransactionId = paymentIntent.RazorpayOrderId;

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentIntent.RazorpayOrderId,
            ClientSecret = paymentIntent.KeyId, // Razorpay Key ID for frontend
            Amount = amountToCharge,
            Currency = paymentIntent.Currency,
            Status = PaymentStatus.Pending,
            RazorpayOrderId = paymentIntent.RazorpayOrderId
        };
    }
}