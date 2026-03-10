using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

public class CreateOrderPaymentIntentHandler
    : IRequestHandler<CreateOrderPaymentIntentCommand, PaymentIntentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPaymentGatewayFactory _gatewayFactory;

    public CreateOrderPaymentIntentHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPaymentGatewayFactory gatewayFactory)
    {
        _context = context;
        _currentUser = currentUser;
        _gatewayFactory = gatewayFactory;
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

        // Get payment gateway (specified or default)
        var gateway = request.Gateway.HasValue
            ? _gatewayFactory.GetGateway(request.Gateway.Value)
            : _gatewayFactory.GetDefaultGateway();

        // Create payment via generic interface
        var paymentResult = await gateway.CreatePaymentAsync(new CreatePaymentRequest
        {
            Amount = amountToCharge,
            Currency = "INR",
            ReferenceId = order.Id,
            ReferenceType = PaymentReferenceType.Order,
            CustomerEmail = order.User?.Email,
            CustomerPhone = order.User?.PhoneNumber,
            CustomerName = $"{order.User?.FirstName} {order.User?.LastName}".Trim(),
            Description = $"Order #{order.OrderNumber}"
        }, cancellationToken);

        if (!paymentResult.Success)
            throw new ValidationException(paymentResult.ErrorMessage ?? "Failed to create payment");

        // Update order payment status
        order.PaymentStatus = PaymentStatus.Pending;
        order.PaymentGateway = gateway.GatewayType;
        order.PaymentTransactionId = paymentResult.GatewayOrderId;

        // For COD, set payment method directly
        if (gateway.GatewayType == PaymentGateway.Manual)
        {
            order.PaymentMethod = PaymentMethod.CashOnDelivery;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Extract client secret from provider-specific data
        var clientSecret = GetClientDataValue(paymentResult.ClientData, "client_secret")
                        ?? GetClientDataValue(paymentResult.ClientData, "key_id")
                        ?? string.Empty;

        return new PaymentIntentDto
        {
            PaymentIntentId = paymentResult.GatewayOrderId,
            ClientSecret = clientSecret,
            Amount = amountToCharge,
            Currency = paymentResult.Currency,
            Status = PaymentStatus.Pending.ToString(),
            Gateway = gateway.GatewayType.ToString(),
            GatewayOrderId = paymentResult.GatewayOrderId,
            ClientData = paymentResult.ClientData
        };
    }

    private static string? GetClientDataValue(IDictionary<string, string> data, string key)
    {
        return data.TryGetValue(key, out var value) ? value : null;
    }
}
