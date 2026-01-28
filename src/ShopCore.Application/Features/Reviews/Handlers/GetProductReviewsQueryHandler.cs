using ShopCore.Application.Common.Models;
using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, PaginatedList<ReviewDto>>
{
    public Task<PaginatedList<ReviewDto>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
