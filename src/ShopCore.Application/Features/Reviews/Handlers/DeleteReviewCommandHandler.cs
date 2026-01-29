namespace ShopCore.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
{
    public Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find review by id
        // 3. Verify current user owns the review
        // 4. Delete all review images
        // 5. Remove review from database
        // 6. Update product rating/review count
        // 7. Save changes
        return Task.CompletedTask;
    }
}
