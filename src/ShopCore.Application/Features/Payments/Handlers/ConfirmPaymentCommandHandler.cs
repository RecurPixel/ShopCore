using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

public class ConfirmPaymentCommandHandler
    : IRequestHandler<ConfirmPaymentCommand, PaymentConfirmationDto>
{
    public Task<PaymentConfirmationDto> Handle(
        ConfirmPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new PaymentConfirmationDto());
    }
}
