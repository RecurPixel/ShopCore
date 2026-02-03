using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressDto> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == _currentUser.UserId && !a.IsDeleted, ct);

        if (address == null)
            throw new NotFoundException("Address", request.Id);

        // If this is set as default, unset other defaults
        if (request.IsDefault && !address.IsDefault)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == _currentUser.UserId && a.IsDefault && a.Id != request.Id)
                .ToListAsync(ct);

            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
            }
        }

        address.FullName = request.FullName;
        address.PhoneNumber = request.PhoneNumber;
        address.AddressLine1 = request.AddressLine1;
        address.AddressLine2 = request.AddressLine2;
        address.City = request.City;
        address.State = request.State;
        address.Country = request.Country;
        address.Pincode = request.Pincode;
        address.IsDefault = request.IsDefault;

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
