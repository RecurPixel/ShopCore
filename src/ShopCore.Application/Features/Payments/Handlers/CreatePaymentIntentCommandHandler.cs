using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommandHandler
    : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentDto>
{
    public Task<PaymentIntentDto> Handle(
        CreatePaymentIntentCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new PaymentIntentDto());
    }
}
