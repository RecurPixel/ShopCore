using ShopCore.Application.VendorServiceAreas.Commands.RemoveVendorServiceArea;

namespace ShopCore.Application.VendorServiceAreas.Handlers;

public class RemoveVendorServiceAreaCommandHandler
    : IRequestHandler<RemoveVendorServiceAreaCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RemoveVendorServiceAreaCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        RemoveVendorServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        var serviceArea = await _context.VendorServiceAreas
            .FirstOrDefaultAsync(sa => sa.Id == request.ServiceAreaId && sa.VendorId == vendor.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorServiceArea), request.ServiceAreaId);

        serviceArea.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
