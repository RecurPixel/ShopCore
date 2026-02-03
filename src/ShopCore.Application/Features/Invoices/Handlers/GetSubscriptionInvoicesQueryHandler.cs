using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

public class GetSubscriptionInvoicesQueryHandler
    : IRequestHandler<GetSubscriptionInvoicesQuery, PaginatedList<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSubscriptionInvoicesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<InvoiceDto>> Handle(
        GetSubscriptionInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.SubscriptionInvoices
            .AsNoTracking()
            .Include(i => i.Subscription)
            .Where(i => i.SubscriptionId == request.SubscriptionId
                && (i.UserId == _currentUser.UserId || i.VendorId == _currentUser.VendorId));

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status)
            && Enum.TryParse<InvoiceStatus>(request.Status, out var status))
        {
            query = query.Where(i => i.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(i => i.GeneratedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                SubscriptionId = i.SubscriptionId,
                GeneratedAt = i.GeneratedAt,
                DueDate = i.DueDate,
                PaidAt = i.PaidAt,
                PeriodStart = i.PeriodStart,
                PeriodEnd = i.PeriodEnd,
                TotalDeliveries = i.TotalDeliveries,
                Subtotal = i.Subtotal,
                Tax = i.Tax,
                Total = i.Total,
                PaidAmount = i.PaidAmount,
                BalanceDue = i.BalanceDue,
                Status = i.Status.ToString(),
                IsOverdue = i.IsOverdue,
                IsFullyPaid = i.IsFullyPaid,
                PaymentMethod = i.PaymentMethod
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<InvoiceDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}