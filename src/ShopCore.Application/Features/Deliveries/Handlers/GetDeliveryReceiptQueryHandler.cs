using ShopCore.Application.Common.Models;

namespace ShopCore.Application.Deliveries.Queries.GetDeliveryReceipt;

public class GetDeliveryReceiptQueryHandler : IRequestHandler<GetDeliveryReceiptQuery, FileResponse>
{
    public Task<FileResponse> Handle(
        GetDeliveryReceiptQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
