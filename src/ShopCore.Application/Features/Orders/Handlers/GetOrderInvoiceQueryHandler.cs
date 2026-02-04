namespace ShopCore.Application.Orders.Queries.GetOrderInvoice;

public class GetOrderInvoiceQueryHandler : IRequestHandler<GetOrderInvoiceQuery, FileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPdfService _pdfService;

    public GetOrderInvoiceQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPdfService pdfService)
    {
        _context = context;
        _currentUser = currentUser;
        _pdfService = pdfService;
    }

    public async Task<FileResponse> Handle(
        GetOrderInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.ShippingAddress)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Vendor)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == _currentUser.UserId,
                cancellationToken);

        if (order == null)
            throw new NotFoundException("Order not found");

        // Generate PDF invoice
        var pdfBytes = await _pdfService.GenerateOrderInvoicePdfAsync(order);

        return new FileResponse
        {
            FileName = $"Invoice_{order.OrderNumber}.pdf",
            ContentType = "application/pdf",
            Content = pdfBytes
        };
    }
}