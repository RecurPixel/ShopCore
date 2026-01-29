using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    public Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find review by id
        // 3. Verify current user owns the review
        // 4. Update rating, title, comment
        // 5. Handle image updates/deletions if needed
        // 6. Save changes to database
        // 7. Update product rating if changed
        // 8. Map and return updated ReviewDto
        return Task.FromResult(new ReviewDto());
    }
}
