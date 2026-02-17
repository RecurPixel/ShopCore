using ShopCore.Application.Common.Models;
using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public record GetProductReviewsQuery(
    int ProductId,
    string? SortBy = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ReviewDto>>;
