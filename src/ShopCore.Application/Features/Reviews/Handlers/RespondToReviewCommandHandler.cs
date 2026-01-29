using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.RespondToReview;

public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, ReviewDto>
{
    public Task<ReviewDto> Handle(
        RespondToReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current vendor from context
        // 2. Find review by id
        // 3. Verify review is for vendor's product
        // 4. Create vendor response
        // 5. Save response to database
        // 6. Mark review as responded
        // 7. Fetch and return updated ReviewDto
        return Task.FromResult(new ReviewDto());
    }
}
