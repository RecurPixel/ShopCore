namespace ShopCore.Application.Reviews.Commands.MarkReviewHelpful;

public class MarkReviewHelpfulCommandHandler : IRequestHandler<MarkReviewHelpfulCommand>
{
    public Task Handle(MarkReviewHelpfulCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find review by id
        // 3. Check if user has already marked as helpful
        // 4. Add/remove helpful mark
        // 5. Update helpful count on review
        // 6. Save changes to database
        return Task.CompletedTask;
    }
}
