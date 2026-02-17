using ShopCore.Application.Wishlist.DTOs;

namespace ShopCore.Application.Wishlist.Queries.GetWishlist;

public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, WishlistDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWishlistQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<WishlistDto> Handle(
        GetWishlistQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Wishlists
            .AsNoTracking()
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Where(w => w.UserId == _currentUser.UserId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductImageUrl = w.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? w.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                    : null,
                Price = w.Product.Price,
                CompareAtPrice = w.Product.CompareAtPrice,
                IsInStock = w.Product.IsInStock,
                AddedAt = w.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new WishlistDto
        {
            UserId = _currentUser.UserId!.Value,
            Items = items,
            TotalItems = totalCount
        };
    }
}
