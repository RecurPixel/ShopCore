namespace ShopCore.Application.Addresses.Commands.SetDefaultAddress;

public class SetDefaultAddressCommandHandler : IRequestHandler<SetDefaultAddressCommand>
{
    public Task Handle(SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
