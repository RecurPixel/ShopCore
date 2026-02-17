using ShopCore.Application.Products.Commands.CreateProduct;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProduct;

// Note: Status changes via UpdateProductStatusCommand
// Note: IsFeatured changes via FeatureProductCommand (admin only)
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
    bool IsSubscriptionAvailable,
    decimal? SubscriptionDiscount,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    int CategoryId,
    List<ProductSpecInput>? Specifications = null
) : IRequest<ProductDto>;
