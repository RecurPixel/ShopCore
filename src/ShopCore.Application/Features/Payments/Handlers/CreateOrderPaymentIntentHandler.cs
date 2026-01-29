using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

public class CreateOrderPaymentIntentHandler
    : IRequestHandler<CreateOrderPaymentIntentCommand, PaymentIntentDto>
{
    private readonly IApplicationDbContext _context;

    public CreateOrderPaymentIntentHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentIntentDto> Handle(
        CreateOrderPaymentIntentCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            throw new NotFoundException("Order not found");

        // Business logic for order payment
        var amount = order.Total;
        var currency = "USD"; // Assume USD for simplicity

        return new PaymentIntentDto
        {
            PaymentIntentId = Guid.NewGuid().ToString(), // Simulated payment intent ID
            ClientSecret = Guid.NewGuid().ToString(), // Simulated client secret
            Amount = amount,
            Currency = currency,
            Status = PaymentStatus.Unpaid
        };
    }

}