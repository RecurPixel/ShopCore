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

    public async Task<PaginatedList<ReviewDto>> Handle(
        GetMyReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Reviews
            .Where(r => r.UserId == _currentUser.UserId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReviewDto(
                r.Id,
                r.ProductId,
                r.Product.Name,
                r.UserId,
                r.User.FirstName + " " + r.User.LastName,
                r.Rating,
                r.Title,
                r.Comment,
                r.ImageUrls,
                r.VendorResponse,
                r.VendorRespondedAt,
                r.HelpfulCount,
                r.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedList<ReviewDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }
}
