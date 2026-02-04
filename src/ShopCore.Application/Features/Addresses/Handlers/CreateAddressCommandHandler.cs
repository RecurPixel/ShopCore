using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken ct)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        if (user == null)
            throw new NotFoundException("User", _currentUser.UserId);

        // If this is set as default, unset other defaults
        if (request.IsDefault)
        {
            var existingAddresses = await _context.Addresses
                .Where(a => a.UserId == _currentUser.UserId && a.IsDefault)
                .ToListAsync(ct);

            foreach (var addr in existingAddresses)
            {
                addr.IsDefault = false;
            }
        }

        // Check if user has no addresses - make first one default
        var hasAddresses = await _context.Addresses
            .AnyAsync(a => a.UserId == _currentUser.UserId && !a.IsDeleted, ct);

        var address = new Address
        {
            UserId = _currentUser.RequiredUserId,
            User = user,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Pincode = request.Pincode,
            IsDefault = request.IsDefault || !hasAddresses,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PlaceId = request.PlaceId,
            Type = request.Type,
            Landmark = request.Landmark
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(ct);

        return new AddressDto
        {
            Id = address.Id,
            FullName = address.FullName,
            PhoneNumber = address.PhoneNumber,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            Pincode = address.Pincode,
            IsDefault = address.IsDefault
        };
    }
}
