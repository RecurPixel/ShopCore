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

    // GET /api/v1/invoices/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (invoice is null)
            return NotFound();
        return Ok(invoice);
    }

    // GET /api/v1/invoices/{id}/download
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        var pdf = await _mediator.Send(new DownloadInvoiceQuery(id));
        if (pdf is null || pdf.Length == 0)
            return NotFound();
        return File(pdf, "application/pdf", $"invoice-{id}.pdf");
    }
}
