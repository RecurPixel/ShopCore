using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.CreateProduct;

// Note: Status defaults to Draft (use UpdateProductStatusCommand to change)
// Note: IsFeatured defaults to false (use FeatureProductCommand - admin only)
public record CreateProductCommand(
    string Name,
    string Slug,
    string? Description,
    string? ShortDescription,
    decimal Price,
    decimal? CompareAtPrice,
    decimal? CostPerItem,
    int StockQuantity,
    string? SKU,
    string? Barcode,
    bool TrackInventory,
    decimal? Weight,
    string? WeightUnit,
    string? Dimensions,
    bool IsSubscriptionAvailable,
    decimal? SubscriptionDiscount,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    int CategoryId,
    List<ProductSpecInput>? Specifications = null
) : IRequest<ProductDto>;

public record ProductSpecInput(
    string Name,
    string Value
);
