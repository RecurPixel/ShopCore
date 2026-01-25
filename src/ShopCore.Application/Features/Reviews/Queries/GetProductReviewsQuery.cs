using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public record GetProductReviewsQuery : IRequest<List<ReviewDto>>;
