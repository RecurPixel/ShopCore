namespace ShopCore.Application.Products.DTOs;

public record ProductSpecificationDto
{
    public string Name { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}
