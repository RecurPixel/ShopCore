namespace ShopCore.Application.Categories.Commands.UploadCategoryImage;

public class UploadCategoryImageCommandHandler : IRequestHandler<UploadCategoryImageCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadCategoryImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(UploadCategoryImageCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can upload category images");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct);

        if (category == null)
            throw new NotFoundException("Category", request.CategoryId);

        // Delete old image if exists
        if (!string.IsNullOrEmpty(category.ImageUrl))
        {
            await _fileStorage.DeleteFileAsync(category.ImageUrl);
        }

        // Upload new image
        using var stream = request.AvatarFile.OpenReadStream();
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.AvatarFile.FileName)}";
        var imageUrl = await _fileStorage.SaveFileAsync(stream, fileName, $"categories/{request.CategoryId}");

        // Update category
        category.ImageUrl = imageUrl;
        await _context.SaveChangesAsync(ct);

        return _fileStorage.GetFileUrl(imageUrl);
    }
}
