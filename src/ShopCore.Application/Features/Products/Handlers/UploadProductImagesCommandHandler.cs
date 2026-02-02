using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UploadProductImages;

public class UploadProductImagesCommandHandler
    : IRequestHandler<UploadProductImagesCommand, List<ProductImageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadProductImagesCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<List<ProductImageDto>> Handle(UploadProductImagesCommand request, CancellationToken ct)
    {
        // 1. Find product and verify ownership
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        if (product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only upload images for your own products");

        // 2. Get current max display order
        var maxOrder = await _context.ProductImages
            .Where(pi => pi.ProductId == request.ProductId && !pi.IsDeleted)
            .MaxAsync(pi => (int?)pi.DisplayOrder, ct) ?? 0;

        // 3. Upload images and create records
        var images = new List<ProductImage>();
        var currentOrder = maxOrder + 1;

        foreach (var file in request.Images)
        {
            // Upload to storage
            var imageUrl = await _fileStorage.UploadFileAsync(
                file,
                $"products/{request.ProductId}",
                ct);

            // Create image record
            var image = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = imageUrl,
                IsPrimary = images.Count == 0 && maxOrder == 0, // First image is primary if no images exist
                DisplayOrder = currentOrder++
            };

            images.Add(image);
        }

        _context.ProductImages.AddRange(images);
        await _context.SaveChangesAsync(ct);

        return images.Select(i => new ProductImageDto
        {
            Id = i.Id,
            ImageUrl = i.ImageUrl,
            IsPrimary = i.IsPrimary,
            DisplayOrder = i.DisplayOrder
        }).ToList();
    }
}
