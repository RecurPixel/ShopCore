using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorById;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorPublicProfileDto?>
{
    private readonly IApplicationDbContext _context;

    public GetVendorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VendorPublicProfileDto?> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Where(v => v.Id == request.Id && v.Status == VendorStatus.Active)
            .Select(v => new VendorPublicProfileDto
            {
                Id = v.Id,
                BusinessName = v.BusinessName,
                BusinessDescription = v.BusinessDescription,
                BusinessLogo = v.BusinessLogo,
                BusinessAddress = v.BusinessAddress,
                PhoneNumber = v.User.PhoneNumber,
                AverageRating = v.AverageRating,
                TotalReviews = v.TotalReviews,
                TotalProducts = v.TotalProducts,
                IsDeliveryAvailable = v.RequiresDeposit,
                RequiresDeposit = v.RequiresDeposit,
                DefaultDepositAmount = v.DefaultDepositAmount,
                MemberSince = v.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
