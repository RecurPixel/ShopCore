namespace ShopCore.Application.Products.DTOs;

public record ProductImageDto
{
    public int Id { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
    public int DisplayOrder { get; init; }
}
