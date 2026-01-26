using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public record GetProductReviewsQuery(int ProductId) : IRequest<List<ReviewDto>>;
