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
        return Task.FromResult(new ReviewDto());
    }
}
