using ShopCore.Application.VendorServiceAreas.DTOs;
using ShopCore.Application.VendorServiceAreas.Queries.GetVendorServiceAreas;

namespace ShopCore.Application.VendorServiceAreas.Handlers;

public class GetVendorServiceAreasQueryHandler
    : IRequestHandler<GetVendorServiceAreasQuery, List<VendorServiceAreaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetVendorServiceAreasQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<VendorServiceAreaDto>> Handle(
        GetVendorServiceAreasQuery request,
        CancellationToken cancellationToken)
    {
        var vendorId = request.VendorId;

        // If no vendor ID specified, get current user's vendor
        if (!vendorId.HasValue)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var vendor = await _context.VendorProfiles
                .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
                ?? throw new NotFoundException(nameof(VendorProfile), userId);

            vendorId = vendor.Id;
        }

        var serviceAreas = await _context.VendorServiceAreas
            .Where(sa => sa.VendorId == vendorId)
            .OrderBy(sa => sa.AreaName)
            .Select(sa => new VendorServiceAreaDto
            {
                Id = sa.Id,
                VendorId = sa.VendorId,
                AreaName = sa.AreaName,
                City = sa.City,
                State = sa.State,
                Pincodes = sa.Pincodes,
                IsActive = sa.IsActive,
                CreatedAt = sa.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return serviceAreas;
    }
}
