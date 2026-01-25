namespace ShopCore.Application.Reviews.Commands.RespondToReview;

public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand>
{
    public Task Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
