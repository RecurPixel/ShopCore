using ShopCore.Application.Wishlist.DTOs;

namespace ShopCore.Application.Wishlist.Queries.GetWishlist;

public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, WishlistDto>
{
    public Task<WishlistDto> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult(new WishlistDto());
    }
}
