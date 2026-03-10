using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    int ProductId,
    int OrderId,
    int Rating,
    string? Title,
    string? Comment,
    List<IFile>? Images
) : IRequest<ReviewDto>;
