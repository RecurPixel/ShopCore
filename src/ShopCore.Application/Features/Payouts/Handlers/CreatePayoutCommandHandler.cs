using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CreatePayout;

public class CreatePayoutCommandHandler : IRequestHandler<CreatePayoutCommand, PayoutDto>
{
    public Task<PayoutDto> Handle(CreatePayoutCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
