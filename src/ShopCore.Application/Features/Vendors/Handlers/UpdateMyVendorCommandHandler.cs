using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateMyVendor;

public class UpdateMyVendorCommandHandler : IRequestHandler<UpdateMyVendorCommand, VendorProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMyVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorProfileDto> Handle(
        UpdateMyVendorCommand request,
        CancellationToken ct)
    {
        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == _currentUser.UserId, ct);

        if (vendor == null)
            throw new NotFoundException("Vendor profile not found");

        vendor.BusinessName = request.BusinessName;
        vendor.BusinessDescription = request.BusinessDescription;
        vendor.BusinessLogo = request.BusinessLogo;
        vendor.BusinessAddress = request.BusinessAddress;
        vendor.BankName = request.BankName;
        vendor.BankAccountNumber = request.BankAccountNumber;
        vendor.BankIfscCode = request.BankIfscCode;
        vendor.BankAccountHolderName = request.BankAccountHolderName;
        vendor.RequiresDeposit = request.RequiresDeposit;
        vendor.DefaultDepositAmount = request.DefaultDepositAmount;
        vendor.DefaultBillingCycleDays = request.DefaultBillingCycleDays;

        await _context.SaveChangesAsync(ct);

        return new VendorProfileDto(
            vendor.Id,
            vendor.UserId,
            vendor.BusinessName,
            vendor.BusinessDescription,
            vendor.BusinessLogo,
            vendor.BusinessAddress,
            vendor.GstNumber,
            vendor.PanNumber,
            vendor.BankName,
            vendor.BankAccountNumber,
            vendor.BankIfscCode,
            vendor.BankAccountHolderName,
            vendor.RequiresDeposit,
            vendor.DefaultDepositAmount,
            vendor.DefaultBillingCycleDays,
            vendor.Status,
            vendor.AverageRating,
            vendor.TotalReviews,
            vendor.TotalProducts,
            vendor.TotalOrders,
            vendor.CreatedAt
        );
    }
}
