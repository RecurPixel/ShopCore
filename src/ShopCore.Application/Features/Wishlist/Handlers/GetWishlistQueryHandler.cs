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
        var items = await _context.Wishlists
            .AsNoTracking()
            .Include(w => w.Product)
                .ThenInclude(p => p.Category)
            .Include(w => w.Product)
                .ThenInclude(p => p.Vendor)
            .Include(w => w.Product)
                .ThenInclude(p => p.Images)
            .Where(w => w.UserId == _currentUser.UserId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductImageUrl = w.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Price = w.Product.Price,
                CompareAtPrice = w.Product.CompareAtPrice,
                IsInStock = w.Product.IsInStock,
                AddedAt = w.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new WishlistDto
        {
            Items = items,
            TotalItems = items.Count
        };
    }
}
