using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetVendorInvoices;

public class GetVendorInvoicesQueryHandler : IRequestHandler<GetVendorInvoicesQuery, PaginatedList<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorInvoicesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<InvoiceDto>> Handle(
        GetVendorInvoicesQuery request,
        CancellationToken ct)
    {
        var query = _context.SubscriptionInvoices
            .AsNoTracking()
            .Where(i => i.VendorId == _currentUser.VendorId)
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
                .ThenInclude(d => d.Items)
                    .ThenInclude(di => di.Product)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<InvoiceStatus>(request.Status, true, out var status))
        {
            query = query.Where(i => i.Status == status);
        }

        var totalCount = await query.CountAsync(ct);

        var invoices = await query
            .OrderByDescending(i => i.GeneratedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = invoices.Select(MapToDto).ToList();

        return new PaginatedList<InvoiceDto>(items, totalCount, request.Page, request.PageSize);
    }

    private static InvoiceDto MapToDto(SubscriptionInvoice invoice)
    {
        PaymentMethod? paymentMethod = null;
        if (!string.IsNullOrEmpty(invoice.PaymentMethod) &&
            Enum.TryParse<PaymentMethod>(invoice.PaymentMethod, out var pm))
        {
            paymentMethod = pm;
        }

        var lineItems = invoice.Deliveries
            .SelectMany(d => d.Items.Select(i => new InvoiceLineItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                d.ScheduledDate,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )))
            .ToList();

        return new InvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.SubscriptionId,
            invoice.Subscription.SubscriptionNumber,
            invoice.GeneratedAt,
            invoice.DueDate,
            invoice.Subtotal,
            invoice.Tax,
            invoice.Total,
            invoice.PaidAmount,
            invoice.BalanceDue,
            invoice.Status,
            invoice.PaidAt,
            paymentMethod,
            invoice.PaymentTransactionId,
            lineItems
        );
    }
}
