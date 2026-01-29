using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    private readonly IApplicationDbContext _context;
    public CreateAddressCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<AddressDto> Handle(
        CreateAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Validate address data (pincode, coordinates, etc.)
        // 3. Create new Address entity
        // 4. Set as default if specified or if user has no default
        // 5. Add to database and save
        // 6. Map and return AddressDto
        return Task.FromResult(new AddressDto());
    }
}
