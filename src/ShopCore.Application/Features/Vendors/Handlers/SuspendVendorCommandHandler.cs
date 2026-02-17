namespace ShopCore.Application.Vendors.Commands.SuspendVendor;

public class SuspendVendorCommandHandler : IRequestHandler<SuspendVendorCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SuspendVendorCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(SuspendVendorCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can suspend vendors");

        var vendor = await _context.VendorProfiles
            .Include(v => v.Products)
            .FirstOrDefaultAsync(v => v.Id == request.VendorId, ct);

        if (vendor == null)
            throw new NotFoundException("Vendor", request.VendorId);

        if (vendor.Status == VendorStatus.Suspended)
            throw new BadRequestException("Vendor is already suspended");

        vendor.Status = VendorStatus.Suspended;

        // Deactivate all vendor products
        foreach (var product in vendor.Products)
        {
            product.Status = ProductStatus.Discontinued;
        }

        await _context.SaveChangesAsync(ct);
    }
}
