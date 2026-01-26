using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new ProductDto());
    }
}
