using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.DownloadInvoice;

public class DownloadInvoiceQueryHandler : IRequestHandler<DownloadInvoiceQuery, InvoiceDownloadDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPdfService _pdfService;

    public DownloadInvoiceQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPdfService pdfService)
    {
        _context = context;
        _currentUser = currentUser;
        _pdfService = pdfService;
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

        // Generate PDF using PdfService
        var pdfBytes = await _pdfService.GenerateSubscriptionInvoicePdfAsync(invoice);
        var fileName = $"Invoice_{invoice.InvoiceNumber}.pdf";

        return new InvoiceDownloadDto
        {
            FileContent = pdfBytes,
            FileName = fileName,
            ContentType = "application/pdf"
        };
    }
}
