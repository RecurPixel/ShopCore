using ShopCore.Application.VendorServiceAreas.Commands.UpdateVendorServiceArea;
using ShopCore.Application.VendorServiceAreas.DTOs;

namespace ShopCore.Application.VendorServiceAreas.Handlers;

public class UpdateVendorServiceAreaCommandHandler
    : IRequestHandler<UpdateVendorServiceAreaCommand, VendorServiceAreaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateVendorServiceAreaCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<VendorServiceAreaDto> Handle(
        UpdateVendorServiceAreaCommand request,
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

        serviceArea.AreaName = request.AreaName;
        serviceArea.Pincodes = request.Pincodes;
        serviceArea.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new VendorServiceAreaDto(
            serviceArea.Id,
            serviceArea.VendorId,
            serviceArea.AreaName,
            serviceArea.City,
            serviceArea.State,
            serviceArea.Pincodes,
            serviceArea.IsActive,
            serviceArea.CreatedAt
        );
    }
}
