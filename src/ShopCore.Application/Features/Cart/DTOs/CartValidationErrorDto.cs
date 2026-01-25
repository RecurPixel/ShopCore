namespace ShopCore.Application.Cart.DTOs;

public record CartValidationErrorDto(
    int? ProductId,
    string ProductName,
    string ErrorCode,
    string ErrorMessage
);
