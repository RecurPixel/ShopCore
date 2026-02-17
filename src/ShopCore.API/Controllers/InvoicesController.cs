using ShopCore.Application.Invoices.DTOs;
using ShopCore.Application.Invoices.Queries.DownloadInvoice;
using ShopCore.Application.Invoices.Queries.GetInvoiceById;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Invoice detail endpoints.
/// Customer invoice viewing is at /api/v1/users/me/subscriptions/{id}/invoices
/// Vendor invoice management is at /api/v1/vendors/me/invoices
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves invoice.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>InvoiceDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (invoice is null)
            return NotFound();
        return Ok(invoice);
    }

    /// <summary>
    /// Downloads invoice.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        var pdf = await _mediator.Send(new DownloadInvoiceQuery(id));
        if (pdf is null || pdf.FileContent.Length == 0)
            return NotFound();
        return File(pdf.FileContent, "application/pdf", $"invoice-{id}.pdf");
    }
}
