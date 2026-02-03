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
        if (!_currentUser.UserId.HasValue)
            throw new UnauthorizedAccessException("User ID is not available");

        var userId = _currentUser.UserId.Value;

        // Check if user already has a vendor profile
        var existingVendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, ct);

        if (existingVendor != null)
            throw new BadRequestException("You already have a vendor profile");

        // Check if GST number is already registered
        var gstExists = await _context.VendorProfiles
            .AnyAsync(v => v.GstNumber == request.GstNumber, ct);

        if (gstExists)
            throw new BadRequestException("GST number is already registered");

        var vendor = new VendorProfile
        {
            UserId = userId,
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

        return new VendorProfileDto
        {
            Id = vendor.Id,
            UserId = vendor.UserId,
            BusinessName = vendor.BusinessName,
            BusinessDescription = vendor.BusinessDescription,
            BusinessLogo = vendor.BusinessLogo,
            BusinessAddress = vendor.BusinessAddress,
            GstNumber = vendor.GstNumber,
            PanNumber = vendor.PanNumber,
            BankName = vendor.BankName,
            BankAccountNumber = vendor.BankAccountNumber,
            BankIfscCode = vendor.BankIfscCode,
            BankAccountHolderName = vendor.BankAccountHolderName,
            RequiresDeposit = vendor.RequiresDeposit,
            DefaultDepositAmount = vendor.DefaultDepositAmount,
            DefaultBillingCycleDays = vendor.DefaultBillingCycleDays,
            Status = vendor.Status.ToString(),
            Rating = vendor.AverageRating,
            TotalReviews = vendor.TotalReviews,
            TotalProducts = vendor.TotalProducts,
            TotalOrders = vendor.TotalOrders,
            CreatedAt = vendor.CreatedAt
        };
    }
}
