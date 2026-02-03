namespace ShopCore.Application.Cart.DTOs;

public record CartValidationErrorDto
{
    public int? ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ErrorCode { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
}
