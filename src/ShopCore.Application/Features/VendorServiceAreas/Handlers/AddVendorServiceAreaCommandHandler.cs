using ShopCore.Application.VendorServiceAreas.Commands.AddVendorServiceArea;
using ShopCore.Application.VendorServiceAreas.DTOs;

namespace ShopCore.Application.VendorServiceAreas.Handlers;

public class AddVendorServiceAreaCommandHandler
    : IRequestHandler<AddVendorServiceAreaCommand, VendorServiceAreaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddVendorServiceAreaCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<VendorServiceAreaDto> Handle(
        AddVendorServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        var serviceArea = new VendorServiceArea
        {
            VendorId = vendor.Id,
            AreaName = request.AreaName,
            City = request.City,
            State = request.State,
            Pincodes = request.Pincodes,
            IsActive = true
        };

        _context.VendorServiceAreas.Add(serviceArea);
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
