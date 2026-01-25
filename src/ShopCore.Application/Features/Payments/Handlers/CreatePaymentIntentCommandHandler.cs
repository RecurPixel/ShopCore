namespace ShopCore.Application.Payments.Commands.CreatePaymentIntent;

public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand>
{
    public Task Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
