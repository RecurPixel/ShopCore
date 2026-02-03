namespace ShopCore.Application.Cart.DTOs;

public record CartValidationResultDto
{
    public bool IsValid { get; init; }
    public List<CartValidationErrorDto> Errors { get; init; } = new();
    public CartDto? ValidatedCart { get; init; }
}