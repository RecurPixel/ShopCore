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

        var reviews = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new
            {
                r.Id,
                r.ProductId,
                ProductName = r.Product.Name,
                r.UserId,
                UserName = r.User.FirstName + " " + r.User.LastName,
                UserAvatar = r.User.AvatarUrl,
                r.Rating,
                r.Title,
                r.Comment,
                r.ImageUrls,
                r.IsVerifiedPurchase,
                r.HelpfulCount,
                r.VendorResponse,
                r.VendorRespondedAt,
                r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var items = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.ProductName,
            UserId = r.UserId,
            UserName = r.UserName,
            UserAvatar = r.UserAvatar,
            Rating = r.Rating,
            Title = r.Title,
            Comment = r.Comment,
            ImageUrls = string.IsNullOrEmpty(r.ImageUrls)
                ? null
                : r.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(url => url.Trim()).ToList(),
            IsVerifiedPurchase = r.IsVerifiedPurchase,
            HelpfulCount = r.HelpfulCount,
            VendorResponse = r.VendorResponse,
            VendorRespondedAt = r.VendorRespondedAt,
            CreatedAt = r.CreatedAt
        }).ToList();

        return new PaginatedList<ReviewDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
