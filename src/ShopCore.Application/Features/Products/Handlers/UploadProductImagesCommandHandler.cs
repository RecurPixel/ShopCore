using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UploadProductImages;

public class UploadProductImagesCommandHandler
    : IRequestHandler<UploadProductImagesCommand, List<ProductImageDto>>
{
    public Task<List<ProductImageDto>> Handle(
        UploadProductImagesCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get product by id
        // 2. Verify vendor owns this product
        // 3. Validate each image file (size, format, dimensions)
        // 4. Set primary image if first upload
        // 5. Upload images to cloud storage (Azure Blob, S3, etc.)
        // 6. Create ProductImage entities with URLs
        // 7. Save to database
        // 8. Map and return List<ProductImageDto>
        return Task.FromResult(new List<ProductImageDto>());
    }
}
