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
        return Task.FromResult(new List<ProductImageDto>());
    }
}
