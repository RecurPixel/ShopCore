using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ValidateCart;

public class ValidateCartCommandHandler
    : IRequestHandler<ValidateCartCommand, CartValidationResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaxService _taxService;

    public ValidateCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ITaxService taxService)
    {
        _context = context;
        _currentUser = currentUser;
        _taxService = taxService;
    }

    public async Task<CartValidationResultDto> Handle(ValidateCartCommand request, CancellationToken ct)
    {
        // 1. Get cart with items
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Vendor)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null || !cart.Items.Any())
            throw new ValidationException("Cart is empty");

        var errors = new List<CartValidationErrorDto>();
        var hasChanges = false;

        // 2. Validate each item
        foreach (var item in cart.Items.Where(i => !i.IsDeleted))
        {
            var product = item.Product;

            // Check if product still exists and is active
            if (product.IsDeleted || product.Status != ProductStatus.Active)
            {
                errors.Add(new CartValidationErrorDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ErrorCode = "PRODUCT_UNAVAILABLE",
                    ErrorMessage = $"{product.Name} is no longer available"
                });
                continue;
            }

            // Check stock
            if (product.TrackInventory && product.StockQuantity < item.Quantity)
            {
                errors.Add(new CartValidationErrorDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ErrorCode = "INSUFFICIENT_STOCK",
                    ErrorMessage = $"Only {product.StockQuantity} units available (requested {item.Quantity})"
                });
            }

            // Check price changes and auto-update
            if (item.Price != product.Price)
            {
                item.Price = product.Price;
                hasChanges = true;
            }
        }

        // Save price updates if any
        if (hasChanges)
            await _context.SaveChangesAsync(ct);

        // Build validated cart DTO
        CartDto? validatedCart = null;
        if (errors.Count == 0)
        {
            validatedCart = BuildCartDto(cart);
        }

        return new CartValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            ValidatedCart = validatedCart
        };
    }

    private CartDto BuildCartDto(Domain.Entities.Cart cart)
    {
        var items = cart.Items
            .Where(i => !i.IsDeleted && !i.Product.IsDeleted && i.Product.Status == ProductStatus.Active)
            .Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                Price = i.Price,
                Quantity = i.Quantity,
                Subtotal = i.Quantity * i.Price,
                IsInStock = !i.Product.TrackInventory || i.Product.StockQuantity >= i.Quantity,
                VendorId = i.Product.VendorId,
                VendorName = i.Product.Vendor.BusinessName
            }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var discount = cart.Discount ?? 0;
        var tax = _taxService.CalculateTax(subtotal, discount);

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            Total = subtotal + tax - discount,
            ItemCount = items.Sum(i => i.Quantity),
            AppliedCouponCode = cart.AppliedCouponCode
        };
    }
}
