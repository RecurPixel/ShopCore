using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ValidateCart;

public class ValidateCartCommandHandler
    : IRequestHandler<ValidateCartCommand, CartValidationResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ValidateCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartValidationResultDto> Handle(ValidateCartCommand request, CancellationToken ct)
    {
        // 1. Get cart with items
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null || !cart.Items.Any())
            throw new ValidationException("Cart is empty");

        var errors = new List<string>();
        var warnings = new List<string>();

        // 2. Validate each item
        foreach (var item in cart.Items)
        {
            var product = item.Product;

            // Check if product still exists and is active
            if (product.IsDeleted || product.Status != ProductStatus.Active)
            {
                errors.Add($"{product.Name} is no longer available");
                continue;
            }

            // Check stock
            if (product.TrackInventory && product.StockQuantity < item.Quantity)
            {
                errors.Add($"{product.Name}: Only {product.StockQuantity} units available (requested {item.Quantity})");
            }

            // Check price changes
            if (item.Price != product.Price)
            {
                warnings.Add($"{product.Name}: Price changed from ₹{item.Price} to ₹{product.Price}");
                // Auto-update price
                item.Price = product.Price;
            }
        }

        if (errors.Any())
            await _context.SaveChangesAsync(ct); // Save price updates if any

        return new CartValidationResultDto
        {
            IsValid = !errors.Any(),
            Errors = errors,
            Warnings = warnings
        };
    }
}
