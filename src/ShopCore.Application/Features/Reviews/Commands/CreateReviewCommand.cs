using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    int ProductId,
    int? OrderItemId,
    int Rating,
    string? Title,
    string Comment,
    string? ImageUrls
) : IRequest<ReviewDto>;
