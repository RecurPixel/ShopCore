namespace ShopCore.Application.Products.Commands.FeatureProduct;

public record FeatureProductCommand(
    int ProductId,
    bool IsFeatured
) : IRequest;
