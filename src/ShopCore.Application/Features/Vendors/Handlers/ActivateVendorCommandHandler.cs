namespace ShopCore.Application.Vendors.Commands.ActivateVendor;

public class ActivateVendorCommandHandler : IRequestHandler<ActivateVendorCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivateVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ActivateVendorCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can activate vendors");

        var vendor = await _context.VendorProfiles
            .Include(v => v.Products)
            .FirstOrDefaultAsync(v => v.Id == request.VendorId, ct);

        if (vendor == null)
            throw new NotFoundException("Vendor", request.VendorId);

        if (vendor.Status == VendorStatus.Active)
            throw new BadRequestException("Vendor is already active");

        if (vendor.Status == VendorStatus.PendingApproval)
            throw new BadRequestException("Vendor must be approved first, not activated");

        vendor.Status = VendorStatus.Active;

        // Reactivate vendor products
        foreach (var product in vendor.Products.Where(p => !p.IsDeleted))
        {
            product.Status = ProductStatus.Active;
        }

        await _context.SaveChangesAsync(ct);
    }
}
