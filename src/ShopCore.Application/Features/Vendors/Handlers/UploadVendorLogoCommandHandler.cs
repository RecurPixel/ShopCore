namespace ShopCore.Application.Vendors.Commands.UploadVendorLogo;

public class UploadVendorLogoCommandHandler : IRequestHandler<UploadVendorLogoCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadVendorLogoCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(UploadVendorLogoCommand request, CancellationToken ct)
    {
        var vendorProfile = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == _currentUser.UserId, ct);

        if (vendorProfile == null)
            throw new NotFoundException("VendorProfile", _currentUser.UserId);

        // Delete old logo if exists
        if (!string.IsNullOrEmpty(vendorProfile.BusinessLogo))
        {
            await _fileStorage.DeleteFileAsync(vendorProfile.BusinessLogo);
        }

        // Upload new logo
        using var stream = request.AvatarFile.OpenReadStream();
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.AvatarFile.FileName)}";
        var logoUrl = await _fileStorage.SaveFileAsync(stream, fileName, $"vendors/{vendorProfile.Id}/logo");

        // Update vendor profile
        vendorProfile.BusinessLogo = logoUrl;
        await _context.SaveChangesAsync(ct);

        return _fileStorage.GetFileUrl(logoUrl);
    }
}
