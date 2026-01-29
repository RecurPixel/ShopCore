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
        // 1. Get product by id
        // 2. Fetch reviews for product
        // 3. Filter by approved/verified only
        // 4. Sort by helpful count and creation date
        // 5. Apply pagination
        // 6. Include vendor responses
        // 7. Map to ReviewDto and return PaginatedList
        return Task.FromResult(new PaginatedList<ReviewDto>([], 0, request.Page, request.PageSize));
    }
}
