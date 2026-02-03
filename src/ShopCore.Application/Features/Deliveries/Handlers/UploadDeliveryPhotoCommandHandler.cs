namespace ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;

public class UploadDeliveryPhotoCommandHandler : IRequestHandler<UploadDeliveryPhotoCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadDeliveryPhotoCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(
        UploadDeliveryPhotoCommand request,
        CancellationToken ct)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Subscription)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, ct);

        if (delivery == null)
            throw new NotFoundException("Delivery", request.DeliveryId);

        // Verify vendor owns this delivery
        if (delivery.Subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only upload photos for your own deliveries");

        // Validate file
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(request.Photo.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new BadRequestException("Only JPG, PNG, and WebP images are allowed");

        // Upload photo
        var fileName = $"deliveries/{delivery.Id}/proof_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
        var photoUrl = await _fileStorage.UploadFileAsync(request.Photo, fileName, ct);

        // Update delivery
        delivery.DeliveryPhotoUrl = photoUrl;

        await _context.SaveChangesAsync(ct);

        return photoUrl;
    }
}
