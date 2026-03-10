using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorOrderItems;

public class GetVendorOrderItemsQueryHandler : IRequestHandler<GetVendorOrderItemsQuery, List<VendorOrderItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorOrderItemsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<VendorOrderItemDto>> Handle(
        GetVendorOrderItemsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .Include(oi => oi.Product)
                .ThenInclude(p => p.Images)
            .Where(oi => oi.OrderId == request.OrderId
                && oi.VendorId == _currentUser.VendorId)
            .Select(oi => new VendorOrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                ProductSKU = oi.ProductSKU,
                ProductImageUrl = oi.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.Quantity * oi.UnitPrice,
                CommissionRate = oi.CommissionRate,
                CommissionAmount = oi.Quantity * oi.UnitPrice * oi.CommissionRate / 100,
                VendorAmount = oi.Quantity * oi.UnitPrice * (1 - oi.CommissionRate / 100),
                Status = oi.Status.ToString()
            })
            .ToListAsync(cancellationToken);
    }
}
