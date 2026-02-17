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

    /// <summary>
    /// Retrieves delivery.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>DeliveryDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetDeliveryById(int id)
    {
        var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));
        if (delivery is null)
            return NotFound();
        return Ok(delivery);
    }

    /// <summary>
    /// Downloads delivery receipt.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("{id:int}/download-receipt")]
    public async Task<IActionResult> DownloadDeliveryReceipt(int id)
    {
        var receipt = await _mediator.Send(new GetDeliveryReceiptQuery(id));
        if (receipt.Content is null || receipt.FileSize == 0)
            return NotFound();
        return File(receipt.Content, "application/pdf", $"delivery-receipt-{id}.pdf");
    }
}
