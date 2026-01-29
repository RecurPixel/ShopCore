using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current vendor from context
        // 2. Validate product data (name, category, pricing, etc.)
        // 3. Create Product entity
        // 4. Associate with vendor
        // 5. Upload product images if provided
        // 6. Set default status (inactive/pending review)
        // 7. Save to database
        // 8. Map and return ProductDto
        return Task.FromResult(new ProductDto());
    }
}
