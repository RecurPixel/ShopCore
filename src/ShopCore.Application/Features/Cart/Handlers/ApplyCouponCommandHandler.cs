using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ApplyCoupon;

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public ApplyCouponCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<CartDto> Handle(ApplyCouponCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null || !cart.Items.Any())
            throw new ValidationException("Cart is empty");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == request.CouponCode.ToUpperInvariant(), ct);

        if (coupon == null || !coupon.IsActive)
            throw new ValidationException("Invalid coupon code");

        // Validate coupon
        if (_dateTime.UtcNow < coupon.ValidFrom || _dateTime.UtcNow > coupon.ValidUntil)
            throw new ValidationException("Coupon has expired");

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit)
            throw new ValidationException("Coupon usage limit reached");

        var subtotal = cart.Items.Sum(i => i.Quantity * i.Price);

        if (coupon.MinOrderValue.HasValue && subtotal < coupon.MinOrderValue)
            throw new ValidationException($"Minimum order value ₹{coupon.MinOrderValue} required");

        // Calculate discount
        decimal discount = 0;
        if (coupon.Type == CouponType.Percentage)
        {
            discount = subtotal * (coupon.DiscountPercentage!.Value / 100);
            if (coupon.MaxDiscount.HasValue && discount > coupon.MaxDiscount)
                discount = coupon.MaxDiscount.Value;
        }
        else if (coupon.Type == CouponType.FixedAmount)
        {
            discount = coupon.DiscountAmount!.Value;
        }

        cart.AppliedCouponCode = request.CouponCode;
        cart.Discount = discount;

        await _context.SaveChangesAsync(ct);

        return await BuildCartDtoAsync(cart);
    }

    private Task<CartDto> BuildCartDtoAsync(Domain.Entities.Cart cart)
    {
        var subtotal = cart.Items.Sum(i => i.Quantity * i.Price);
        var tax = (subtotal - (cart.Discount ?? 0)) * 0.18m;
        var total = subtotal + tax - (cart.Discount ?? 0);

        return Task.FromResult(new CartDto
        {
            Id = cart.Id,
            Subtotal = subtotal,
            Tax = tax,
            Discount = cart.Discount ?? 0,
            Total = total,
            AppliedCouponCode = cart.AppliedCouponCode
        });
    }
}
