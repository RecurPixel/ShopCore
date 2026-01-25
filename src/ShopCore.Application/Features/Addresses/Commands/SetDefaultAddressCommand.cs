namespace ShopCore.Application.Addresses.Commands.SetDefaultAddress;

public record SetDefaultAddressCommand : IRequest
{
    private Guid id;

    public SetDefaultAddressCommand(Guid id)
    {
        this.id = id;
    }
}
