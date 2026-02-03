namespace ShopCore.Application.Vendors.Commands.ApproveVendor;

public class ApproveVendorCommandHandler : IRequestHandler<ApproveVendorCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ApproveVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ApproveVendorCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can approve vendors");

        var vendor = await _context.VendorProfiles
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == request.VendorId, ct);

        if (vendor == null)
            throw new NotFoundException("Vendor", request.VendorId);

        if (vendor.Status != VendorStatus.PendingApproval)
            throw new BadRequestException("Vendor is not pending approval");

        vendor.Status = VendorStatus.Active;
        vendor.ApprovedAt = DateTime.UtcNow;
        vendor.ApprovedBy = _currentUser.UserId;

        // Update user role to Vendor
        vendor.User.Role = UserRole.Vendor;

        await _context.SaveChangesAsync(ct);
    }
}
