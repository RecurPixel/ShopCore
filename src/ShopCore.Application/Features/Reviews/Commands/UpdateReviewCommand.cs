using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(
    int Id,
    int Rating,
    string? Title,
    string Comment,
    string? ImageUrls
) : IRequest<ReviewDto>;
