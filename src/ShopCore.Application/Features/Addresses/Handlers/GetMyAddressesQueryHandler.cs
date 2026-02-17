using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, PaginatedList<AddressDto>>
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

    public async Task<PaginatedList<AddressDto>> Handle(
        GetMyAddressesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == _currentUser.UserId);

        // Apply filters
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(a =>
                a.FullName.Contains(request.Search) ||
                a.AddressLine1.Contains(request.Search) ||
                a.City.Contains(request.Search) ||
                a.Pincode.Contains(request.Search));
        }

        if (request.IsDefault.HasValue)
            query = query.Where(a => a.IsDefault == request.IsDefault.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
                IsDefault = a.IsDefault,
            })
            .ToListAsync(cancellationToken);
        return new PaginatedList<AddressDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };

    }
}