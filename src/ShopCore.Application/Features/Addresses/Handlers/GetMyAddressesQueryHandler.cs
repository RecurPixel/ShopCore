using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, List<AddressDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyAddressesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<AddressDto>> Handle(
        GetMyAddressesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == _currentUser.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                UserId = a.UserId,
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
                Landmark = a.Landmark,
                AddressType = a.AddressType,
                IsDefault = a.IsDefault,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}