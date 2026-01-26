using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProductStatus;

public class UpdateProductStatusCommandHandler
    : IRequestHandler<UpdateProductStatusCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        UpdateProductStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new ProductDto());
    }
}
