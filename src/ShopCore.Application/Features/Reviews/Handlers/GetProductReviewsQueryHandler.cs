using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, List<ReviewDto>>
{
    public Task<List<ReviewDto>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<ReviewDto>());
    }
}
