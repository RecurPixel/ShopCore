using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public CreateOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // 1. Get cart with items
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Vendor)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null || !cart.Items.Any())
            throw new ValidationException("Cart is empty");

        // 2. Verify address
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == _currentUser.UserId, ct);

        if (address == null)
            throw new NotFoundException("Address", request.AddressId);

        // 3. Validate stock for all items
        foreach (var item in cart.Items)
        {
            if (item.Product.TrackInventory && item.Product.StockQuantity < item.Quantity)
                throw new ValidationException($"{item.Product.Name}: Insufficient stock");
        }

        // 4. Calculate totals
        var subtotal = cart.Items.Sum(i => i.Quantity * i.Price);
        var tax = subtotal * 0.18m; // 18% GST
        decimal discount = 0;

        // 5. Apply coupon if provided
        Coupon? coupon = null;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == request.CouponCode.ToUpperInvariant(), ct);

            if (coupon == null || !coupon.IsValid)
                throw new ValidationException("Invalid or expired coupon");

            if (coupon.MinOrderValue.HasValue && subtotal < coupon.MinOrderValue)
                throw new ValidationException($"Minimum order value ₹{coupon.MinOrderValue} required");

            // Calculate discount
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

            coupon.UsageCount++;
        }

        var shippingCharge = 0m; // Free shipping for now
        var total = subtotal + tax - discount + shippingCharge;

        // 6. Generate order number
        var orderNumber = await GenerateOrderNumberAsync(ct);

        // 7. Create order
        var order = new Order
        {
            OrderNumber = orderNumber,
            UserId = _currentUser.UserId!.Value,
            ShippingAddressId = request.AddressId,
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            Tax = tax,
            Discount = discount,
            ShippingCharge = shippingCharge,
            Total = total,
            PaymentStatus = PaymentStatus.Unpaid,
            PaymentMethod = request.PaymentMethod,
            CouponId = coupon?.Id,
            CustomerNotes = request.CustomerNotes
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct); // Save to get order ID

        // 8. Create order items (multi-vendor aware)
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.Items)
        {
            // Reduce stock
            if (cartItem.Product.TrackInventory)
            {
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }

            // Get vendor commission rate
            var vendor = cartItem.Product.Vendor;

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                VendorId = cartItem.Product.VendorId,
                ProductName = cartItem.Product.Name,
                ProductSKU = cartItem.Product.SKU,
                ProductImageUrl = cartItem.Product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Price,
                CommissionRate = vendor.CommissionRate,
                Status = OrderStatus.Pending
            };

            orderItems.Add(orderItem);
        }

        _context.OrderItems.AddRange(orderItems);

        // 9. Create status history
        var statusHistory = new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Pending,
            ChangedAt = _dateTime.UtcNow,
            Notes = "Order created"
        };

        _context.OrderStatusHistory.Add(statusHistory);

        // 10. Clear cart
        _context.CartItems.RemoveRange(cart.Items);

        await _context.SaveChangesAsync(ct);

        // 11. Return order DTO
        return await GetOrderDtoAsync(order.Id, ct);
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken ct)
    {
        var today = _dateTime.UtcNow;
        var prefix = $"ORD-{today:yyyy-MMdd}";

        var todayOrders = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .CountAsync(ct);

        return $"{prefix}-{(todayOrders + 1):D4}";
    }

    private async Task<OrderDto> GetOrderDtoAsync(int orderId, CancellationToken ct)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Vendor)
            .Include(o => o.ShippingAddress)
            .FirstAsync(o => o.Id == orderId, ct);

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            Subtotal = order.Subtotal,
            Tax = order.Tax,
            Discount = order.Discount,
            ShippingCharge = order.ShippingCharge,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal,
                Status = i.Status.ToString(),
                VendorId = i.VendorId,
                VendorName = i.Vendor.BusinessName
            }).ToList()
        };
    }
}
