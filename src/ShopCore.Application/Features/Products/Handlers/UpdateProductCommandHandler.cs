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
        // 1. Get current vendor from context
        // 2. Find product by id
        // 3. Verify vendor owns the product
        // 4. Update product properties (name, description, pricing, etc.)
        // 5. Handle image updates if needed
        // 6. Validate updated data
        // 7. Save changes to database
        // 8. Map and return updated ProductDto
        return Task.FromResult(new ProductDto());
    }
}
