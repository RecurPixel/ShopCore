namespace ShopCore.Application.Reviews.Commands.MarkReviewHelpful;

public class MarkReviewHelpfulCommandHandler : IRequestHandler<MarkReviewHelpfulCommand>
{
    public Task Handle(MarkReviewHelpfulCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
