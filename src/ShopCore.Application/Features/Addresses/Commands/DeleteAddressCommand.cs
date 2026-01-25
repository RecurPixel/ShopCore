namespace ShopCore.Application.Addresses.Commands.DeleteAddress;

public record DeleteAddressCommand(int Id) : IRequest<Unit>;
