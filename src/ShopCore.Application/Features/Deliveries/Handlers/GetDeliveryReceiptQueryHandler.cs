namespace ShopCore.Application.Deliveries.Queries.GetDeliveryReceipt;

public class GetDeliveryReceiptQueryHandler : IRequestHandler<GetDeliveryReceiptQuery, FileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IPdfService _pdfService;

    public GetDeliveryReceiptQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IPdfService pdfService)
    {
        _context = context;
        _currentUser = currentUser;
        _pdfService = pdfService;
    }

    public async Task<FileResponse> Handle(
        GetDeliveryReceiptQuery request,
        CancellationToken cancellationToken)
    {
        var delivery = await _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Subscription)
                .ThenInclude(s => s.User)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.Vendor)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
            .Include(d => d.Items)
                .ThenInclude(di => di.Product)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId
                && (d.Subscription.UserId == _currentUser.UserId
                    || d.Subscription.VendorId == _currentUser.VendorId),
                cancellationToken);

        if (delivery == null)
            throw new NotFoundException("Delivery not found");

        // Generate PDF receipt
        var pdfBytes = await _pdfService.GenerateDeliveryReceiptAsync(delivery);

        return new FileResponse
        {
            FileName = $"Delivery_{delivery.DeliveryNumber}.pdf",
            ContentType = "application/pdf",
            Content = pdfBytes
        };
    }
}
