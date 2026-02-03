using ShopCore.Application.Common.Models;
using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetProductReviews;

public class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, PaginatedList<ReviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductReviewsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ReviewDto>> Handle(
        GetProductReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.ProductId == request.ProductId && r.IsApproved);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "helpful" => query.OrderByDescending(r => r.HelpfulCount),
            "recent" => query.OrderByDescending(r => r.CreatedAt),
            "rating-high" => query.OrderByDescending(r => r.Rating),
            "rating-low" => query.OrderBy(r => r.Rating),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                UserId = r.UserId,
                UserName = r.User.FirstName + " " + r.User.LastName,
                UserAvatar = r.User.AvatarUrl,
                Rating = r.Rating,
                Title = r.Title,
                Comment = r.Comment,
                ImageUrls = r.ImageUrls,
                IsVerifiedPurchase = r.IsVerifiedPurchase,
                HelpfulCount = r.HelpfulCount,
                VendorResponse = r.VendorResponse,
                VendorRespondedAt = r.VendorRespondedAt,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ReviewDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
