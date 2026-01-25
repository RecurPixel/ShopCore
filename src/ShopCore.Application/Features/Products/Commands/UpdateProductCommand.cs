using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    int Id,
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
    int CategoryId
) : IRequest<ProductDto>;
