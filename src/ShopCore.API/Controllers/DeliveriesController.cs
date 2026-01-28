using ShopCore.Application.Deliveries.DTOs;
using ShopCore.Application.Deliveries.Queries.GetDeliveryById;
using ShopCore.Application.Deliveries.Queries.GetDeliveryReceipt;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Delivery detail endpoints.
/// Customer delivery viewing is at /api/v1/users/me/subscriptions/{id}/deliveries
/// Vendor delivery management is at /api/v1/vendors/me/deliveries
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/deliveries")]
public class DeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/deliveries/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetDeliveryById(int id)
    {
        var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));
        if (delivery is null)
            return NotFound();
        return Ok(delivery);
    }

    // GET /api/v1/deliveries/{id}/download-receipt
    [HttpGet("{id:int}/download-receipt")]
    public async Task<IActionResult> DownloadDeliveryReceipt(int id)
    {
        var receipt = await _mediator.Send(new GetDeliveryReceiptQuery(id));
        if (receipt.FileContent is null || receipt.Length == 0)
            return NotFound();
        return File(receipt.FileContent, "application/pdf", $"delivery-receipt-{id}.pdf");
    }
}
