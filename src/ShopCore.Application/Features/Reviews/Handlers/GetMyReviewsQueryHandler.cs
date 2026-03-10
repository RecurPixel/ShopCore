using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Queries.GetMyReviews;

public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, PaginatedList<ReviewDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyReviewsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<ReviewDto>> Handle(
        GetMyReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .AsNoTracking()
            .Include(r => r.Product)
                .ThenInclude(p => p.Images)
            .Where(r => r.UserId == _currentUser.UserId)
            .OrderByDescending(r => r.CreatedAt);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                ProductImageUrl = r.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Rating = r.Rating,
                Title = r.Title,
                Comment = r.Comment,
                ImageUrls = string.IsNullOrEmpty(r.ImageUrls) ? new List<string>() : r.ImageUrls.Split(',', StringSplitOptions.None).ToList(),
                IsVerifiedPurchase = r.IsVerifiedPurchase,
                IsApproved = r.IsApproved,
                HelpfulCount = r.HelpfulCount,
                VendorResponse = r.VendorResponse,
                VendorRespondedAt = r.VendorRespondedAt,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ReviewDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems
        };
    }
}
