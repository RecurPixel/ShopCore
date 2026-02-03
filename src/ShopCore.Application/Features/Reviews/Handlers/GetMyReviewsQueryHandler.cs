using ShopCore.Application.Common.Models;
using ShopCore.Application.Reviews.DTOs;
using ShopCore.Application.Reviews.Queries.GetMyReviews;

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

    public async Task<List<ReviewDto>> Handle(
        GetMyReviewsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Include(r => r.Product)
                .ThenInclude(p => p.Images)
            .Where(r => r.UserId == _currentUser.UserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                ProductImage = r.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Rating = r.Rating,
                Title = r.Title,
                Comment = r.Comment,
                ImageUrls = r.ImageUrls,
                IsVerifiedPurchase = r.IsVerifiedPurchase,
                IsApproved = r.IsApproved,
                HelpfulCount = r.HelpfulCount,
                VendorResponse = r.VendorResponse,
                VendorRespondedAt = r.VendorRespondedAt,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
