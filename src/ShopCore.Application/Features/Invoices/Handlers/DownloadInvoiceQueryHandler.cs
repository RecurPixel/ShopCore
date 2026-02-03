using System.Text;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.DownloadInvoice;

public class DownloadInvoiceQueryHandler : IRequestHandler<DownloadInvoiceQuery, InvoiceDownloadDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DownloadInvoiceQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<InvoiceDownloadDto> Handle(
        DownloadInvoiceQuery request,
        CancellationToken ct)
    {
        var invoice = await _context.SubscriptionInvoices
            .AsNoTracking()
            .Include(i => i.User)
            .Include(i => i.Vendor)
            .Include(i => i.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
            .Include(i => i.Deliveries)
                .ThenInclude(d => d.Items)
                    .ThenInclude(di => di.Product)
            .FirstOrDefaultAsync(i => i.Id == request.Id, ct);

        if (invoice == null)
            throw new NotFoundException("Invoice", request.Id);

        // Verify access
        var hasAccess = invoice.UserId == _currentUser.UserId ||
                       invoice.VendorId == _currentUser.VendorId ||
                       _currentUser.Role == UserRole.Admin;

        if (!hasAccess)
            throw new ForbiddenException("You don't have access to this invoice");

        var content = GenerateInvoiceContent(invoice);
        var fileName = $"Invoice_{invoice.InvoiceNumber}.txt";

        return new InvoiceDownloadDto(
            Encoding.UTF8.GetBytes(content),
            fileName,
            "text/plain"
        );
    }

    private static string GenerateInvoiceContent(SubscriptionInvoice invoice)
    {
        var sb = new StringBuilder();

        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("                    SUBSCRIPTION INVOICE");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"Invoice Number: {invoice.InvoiceNumber}");
        sb.AppendLine($"Generated: {invoice.GeneratedAt:dd MMM yyyy}");
        sb.AppendLine($"Due Date: {invoice.DueDate:dd MMM yyyy}");
        sb.AppendLine($"Status: {invoice.Status}");
        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine("VENDOR");
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine($"{invoice.Vendor.BusinessName}");
        sb.AppendLine($"GST: {invoice.Vendor.GstNumber}");
        sb.AppendLine($"{invoice.Vendor.BusinessAddress}");
        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine("CUSTOMER");
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine($"{invoice.User.FullName}");
        sb.AppendLine($"Email: {invoice.User.Email}");
        var addr = invoice.Subscription.DeliveryAddress;
        sb.AppendLine($"{addr.AddressLine1}");
        if (!string.IsNullOrEmpty(addr.AddressLine2))
            sb.AppendLine($"{addr.AddressLine2}");
        sb.AppendLine($"{addr.City}, {addr.State} - {addr.Pincode}");
        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine($"BILLING PERIOD: {invoice.PeriodStart:dd MMM yyyy} - {invoice.PeriodEnd:dd MMM yyyy}");
        sb.AppendLine($"Total Deliveries: {invoice.TotalDeliveries}");
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine();

        foreach (var delivery in invoice.Deliveries.OrderBy(d => d.ScheduledDate))
        {
            sb.AppendLine($"  Delivery: {delivery.ScheduledDate:dd MMM yyyy}");
            foreach (var item in delivery.Items)
            {
                sb.AppendLine($"    - {item.Product.Name} x {item.Quantity} @ ₹{item.UnitPrice:N2} = ₹{item.Quantity * item.UnitPrice:N2}");
            }
            sb.AppendLine($"    Subtotal: ₹{delivery.TotalAmount:N2}");
            sb.AppendLine();
        }

        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine("SUMMARY");
        sb.AppendLine("───────────────────────────────────────────────────────────────");
        sb.AppendLine($"  Subtotal:         ₹{invoice.Subtotal,12:N2}");
        sb.AppendLine($"  Tax (GST 18%):    ₹{invoice.Tax,12:N2}");
        sb.AppendLine("                    ─────────────────");
        sb.AppendLine($"  TOTAL:            ₹{invoice.Total,12:N2}");
        sb.AppendLine($"  Paid:             ₹{invoice.PaidAmount,12:N2}");
        sb.AppendLine($"  Balance Due:      ₹{invoice.BalanceDue,12:N2}");
        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");

        return sb.ToString();
    }
}
