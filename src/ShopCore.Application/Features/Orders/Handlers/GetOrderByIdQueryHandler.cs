using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetOrderByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<OrderDetailDto> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.ShippingAddress)
            .Include(o => o.Coupon)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Vendor)
            .Include(o => o.StatusHistory)
            .Where(o => o.Id == request.OrderId && o.UserId == _currentUser.UserId)
            .Select(o => new OrderDetailDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                CustomerName = o.User.FirstName + " " + o.User.LastName,
                CustomerEmail = o.User.Email,
                CustomerPhone = o.User.PhoneNumber,
                ShippingAddress = new OrderAddressDto
                {
                    FullName = o.ShippingAddress.FullName,
                    PhoneNumber = o.ShippingAddress.PhoneNumber,
                    AddressLine1 = o.ShippingAddress.AddressLine1,
                    AddressLine2 = o.ShippingAddress.AddressLine2,
                    City = o.ShippingAddress.City,
                    State = o.ShippingAddress.State,
                    Pincode = o.ShippingAddress.Pincode,
                    Landmark = o.ShippingAddress.Landmark
                },
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                PaymentTransactionId = o.PaymentTransactionId,
                PaidAt = o.PaidAt,
                Subtotal = o.Subtotal,
                Tax = o.Tax,
                Discount = o.Discount,
                ShippingCharge = o.ShippingCharge,
                Total = o.Total,
                CouponCode = o.Coupon != null ? o.Coupon.Code : null,
                CustomerNotes = o.CustomerNotes,
                AdminNotes = o.AdminNotes,
                DeliveredAt = o.DeliveredAt,
                CancelledAt = o.CancelledAt,
                CancellationReason = o.CancellationReason,
                Items = o.Items.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductSKU = oi.ProductSKU,
                    ProductImage = oi.ProductImageUrl,
                    VendorId = oi.VendorId,
                    VendorName = oi.Vendor.BusinessName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal,
                    CommissionRate = oi.CommissionRate,
                    CommissionAmount = oi.CommissionAmount,
                    VendorAmount = oi.VendorAmount,
                    Status = oi.Status.ToString()
                }).ToList(),
                StatusHistory = o.StatusHistory
                    .OrderBy(sh => sh.ChangedAt)
                    .Select(sh => new OrderStatusHistoryDto
                    {
                        Status = sh.Status.ToString(),
                        Notes = sh.Notes,
                        ChangedAt = sh.ChangedAt
                    }).ToList(),
                CreatedAt = o.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order == null)
            throw new NotFoundException("Order not found");

        return order;
    }
}