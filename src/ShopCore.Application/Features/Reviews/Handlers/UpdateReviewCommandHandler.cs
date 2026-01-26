using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    public Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new ReviewDto());
    }
}
