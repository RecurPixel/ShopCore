using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    public Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Validate order item was purchased by user
        // 3. Check if review already exists for this item
        // 4. Create Review entity with rating, title, comment
        // 5. Upload review images if provided
        // 6. Save review to database
        // 7. Update product rating/review count
        // 8. Map and return ReviewDto
        return Task.FromResult(new ReviewDto());
    }
}
