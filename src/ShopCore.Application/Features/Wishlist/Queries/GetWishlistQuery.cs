using ShopCore.Application.Wishlist.DTOs;

namespace ShopCore.Application.Wishlist.Queries.GetWishlist;

public record GetWishlistQuery(
    int Page = 1,
    int PageSize = 20
) : IRequest<WishlistDto>;
