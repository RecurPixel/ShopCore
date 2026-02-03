using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, AddressDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAddressByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<AddressDto> Handle(
        GetAddressByIdQuery request,
        CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .AsNoTracking()
            .Where(a => a.Id == request.Id && a.UserId == _currentUser.UserId)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                Pincode = a.Pincode,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                PlaceId = a.PlaceId,
                IsDefault = a.IsDefault
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (address == null)
            throw new NotFoundException(nameof(Address), request.Id);

        return address;
    }
}
