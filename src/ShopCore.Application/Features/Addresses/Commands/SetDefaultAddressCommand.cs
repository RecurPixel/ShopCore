namespace ShopCore.Application.Addresses.Commands.SetDefaultAddress;

public record SetDefaultAddressCommand(int Id) : IRequest<Unit>;
