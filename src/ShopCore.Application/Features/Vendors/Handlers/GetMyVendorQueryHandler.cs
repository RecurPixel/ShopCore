using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendor;

public class GetMyVendorQueryHandler : IRequestHandler<GetMyVendorQuery, VendorProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyVendorQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorProfileDto> Handle(
        GetMyVendorQuery request,
        CancellationToken cancellationToken)
    {
        var vendor = await _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Where(v => v.UserId == _currentUser.UserId)
            .Select(v => new VendorProfileDto
            {
                Id = v.Id,
                UserId = v.UserId,
                BusinessName = v.BusinessName,
                BusinessDescription = v.BusinessDescription,
                BusinessLogo = v.BusinessLogo,
                BusinessAddress = v.BusinessAddress,
                GstNumber = v.GstNumber,
                PanNumber = v.PanNumber,
                BankName = v.BankName,
                BankAccountNumber = v.BankAccountNumber,
                BankIfscCode = v.BankIfscCode,
                BankAccountHolderName = v.BankAccountHolderName,
                Email = v.User.Email,
                PhoneNumber = v.User.PhoneNumber,
                Status = v.Status.ToString(),
                CommissionRate = v.CommissionRate,
                RequiresDeposit = v.RequiresDeposit,
                DefaultDepositAmount = v.DefaultDepositAmount,
                DefaultBillingCycleDays = v.DefaultBillingCycleDays,
                Rating = v.AverageRating,
                TotalReviews = v.TotalReviews,
                TotalProducts = v.TotalProducts,
                TotalOrders = v.TotalOrders,
                TotalRevenue = v.TotalRevenue,
                ApprovedAt = v.ApprovedAt,
                CreatedAt = v.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (vendor == null)
            throw new NotFoundException("Vendor profile not found");

        return vendor;
    }
}