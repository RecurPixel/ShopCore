using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.RespondToReview;

public record RespondToReviewCommand(int ReviewId, string VendorResponse) : IRequest<ReviewDto>;
