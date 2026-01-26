using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    public Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new ReviewDto());
    }
}
