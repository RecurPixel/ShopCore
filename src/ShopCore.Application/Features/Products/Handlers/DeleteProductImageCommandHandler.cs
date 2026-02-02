namespace ShopCore.Application.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public DeleteProductImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeleteProductImageCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        if (product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only delete images from your own products");

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image == null)
            throw new NotFoundException("Image", request.ImageId);

        // Delete from storage
        await _fileStorage.DeleteFileAsync(image.ImageUrl);

        // Delete from database
        _context.ProductImages.Remove(image);

        // If this was primary, set another as primary
        if (image.IsPrimary && product.Images.Any(i => i.Id != request.ImageId))
        {
            var nextPrimary = product.Images
                .Where(i => i.Id != request.ImageId)
                .OrderBy(i => i.DisplayOrder)
                .First();
            nextPrimary.IsPrimary = true;
        }

        await _context.SaveChangesAsync(ct);
    }
