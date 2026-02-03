using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.RegisterVendor;

public class RegisterVendorCommandHandler : IRequestHandler<RegisterVendorCommand, VendorProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RegisterVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorProfileDto> Handle(
        RegisterVendorCommand request,
        CancellationToken ct)
    {
        // Check if user already has a vendor profile
        var existingVendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == _currentUser.UserId, ct);

        if (existingVendor != null)
            throw new BadRequestException("You already have a vendor profile");

        // Check if GST number is already registered
        var gstExists = await _context.VendorProfiles
            .AnyAsync(v => v.GstNumber == request.GstNumber, ct);

        if (gstExists)
            throw new BadRequestException("GST number is already registered");

        var vendor = new VendorProfile
        {
            UserId = _currentUser.UserId,
            BusinessName = request.BusinessName,
            BusinessDescription = request.BusinessDescription,
            BusinessLogo = request.BusinessLogo,
            BusinessAddress = request.BusinessAddress,
            GstNumber = request.GstNumber,
            PanNumber = request.PanNumber,
            BankName = request.BankName,
            BankAccountNumber = request.BankAccountNumber,
            BankIfscCode = request.BankIfscCode,
            BankAccountHolderName = request.BankAccountHolderName,
            RequiresDeposit = request.RequiresDeposit,
            DefaultDepositAmount = request.DefaultDepositAmount,
            DefaultBillingCycleDays = request.DefaultBillingCycleDays,
            Status = VendorStatus.PendingApproval
        };

        _context.VendorProfiles.Add(vendor);
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
