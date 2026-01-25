namespace ShopCore.Application.Cart.DTOs;

public record CartValidationResultDto(
    bool IsValid,
    List<CartValidationErrorDto> Errors,
    CartDto? ValidatedCart
);
