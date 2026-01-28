using ShopCore.Application.Common.Models;
using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetMyReviews;

public record GetMyReviewsQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ReviewDto>>;
