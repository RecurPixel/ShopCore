namespace ShopCore.Application.Cart.Commands.ValidateCart;

public class ValidateCartCommandHandler : IRequestHandler<ValidateCartCommand>
{
    public Task Handle(ValidateCartCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
