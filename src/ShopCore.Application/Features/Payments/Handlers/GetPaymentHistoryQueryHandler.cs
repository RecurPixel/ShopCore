using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler
    : IRequestHandler<GetPaymentHistoryQuery, PaginatedList<PaymentHistoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPaymentHistoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<PaymentHistoryDto>> Handle(
        GetPaymentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Combine payments from both orders and subscription invoices
        var orderPayments = _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == _currentUser.UserId
                     && o.PaymentStatus == PaymentStatus.Paid)
            .Select(o => new PaymentHistoryDto
            {
                PaymentId = "ORD-" + o.Id,
                Type = "Order",
                ReferenceNumber = o.OrderNumber,
                Amount = o.Total,
                PaymentMethod = o.PaymentMethod.ToString(),
                TransactionId = o.PaymentTransactionId,
                Status = "Paid",
                PaidAt = o.PaidAt!.Value,
                Description = $"Order payment - {o.Items.Count} items"
            });

        var invoicePayments = _context.SubscriptionInvoices
            .AsNoTracking()
            .Where(i => i.UserId == _currentUser.UserId
                     && i.Status == InvoiceStatus.Paid)
            .Select(i => new PaymentHistoryDto
            {
                PaymentId = "INV-" + i.Id,
                Type = "Invoice",
                ReferenceNumber = i.InvoiceNumber,
                Amount = i.Total,
                PaymentMethod = i.PaymentMethod.ToString(),
                TransactionId = i.PaymentTransactionId,
                Status = "Paid",
                PaidAt = i.PaidAt!.Value,
                Description = $"Subscription invoice - {i.TotalDeliveries} deliveries"
            });

        var combinedQuery = orderPayments.Union(invoicePayments);

        var totalCount = await combinedQuery.CountAsync(cancellationToken);

        var items = await combinedQuery
            .OrderByDescending(p => p.PaidAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<PaymentHistoryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
