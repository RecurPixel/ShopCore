using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.CreateProduct;

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
    ProductStatus Status,
    bool IsFeatured,
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
