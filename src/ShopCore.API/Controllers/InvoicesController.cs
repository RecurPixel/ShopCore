using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;
using ShopCore.Application.Invoices.Commands.PayInvoice;
using ShopCore.Application.Invoices.DTOs;
using ShopCore.Application.Invoices.Queries.DownloadInvoice;
using ShopCore.Application.Invoices.Queries.GetInvoiceById;
using ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // User endpoints
    // ----------------


    // GET /api/v1/invoices/unpaid
    [HttpGet("unpaid")]
    public async Task<ActionResult<List<InvoiceDto>>> GetUnpaidInvoices()
    {
        var invoices = await _mediator.Send(new GetUnpaidInvoicesQuery());
        return Ok(invoices);
    }

    // GET /api/v1/invoices/subscriptions/{id}/invoices
    [Authorize]
    [Authorize(Roles = "Vendor")]
    [HttpGet("subscriptions/{subscriptionId:int}/invoices")]
    public async Task<ActionResult<PaginatedList<InvoiceDto>>> GetSubscriptionInvoices(
        int subscriptionId,
        [FromQuery] GetSubscriptionInvoicesQuery query)
    {
        var finalQuery = query with { SubscriptionId = subscriptionId };

        var invoices = await _mediator.Send(finalQuery);
        return Ok(invoices);
    }

    // GET /api/v1/invoices/{id}
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));

        if (invoice is null)
            return NotFound();

        return Ok(invoice);
    }

    // POST /api/v1/invoices/{id}/pay
    [Authorize]
    [HttpPost("{id}/pay")]
    public async Task<ActionResult<PaymentConfirmationDto>> PayInvoice(
        int id,
        [FromBody] PayInvoiceCommand command)
    {
        var finalCommand = command with { InvoiceId = id };

        var confirmation = await _mediator.Send(finalCommand);
        return NoContent();
    }

    // GET /api/v1/invoices/{id}/download
    [Authorize]
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> DownloadInvoice(int id)
    {
        var pdf = await _mediator.Send(new DownloadInvoiceQuery(id));

        if (pdf is null || pdf.Length == 0)
            return NotFound();

        return File(
            pdf,
            "application/pdf",
            $"invoice-{id}.pdf");
    }

    // ----------------
    // Vendor endpoint
    // ----------------

    // POST /api/v1/invoices/subscriptions/{id}/generate
    [Authorize(Roles = "Vendor")]
    [HttpPost("subscriptions/{id}/generate")]
    public async Task<ActionResult<InvoiceDto>> GenerateInvoice(
        int subscriptionId)
    {
        var invoice = await _mediator.Send(
            new GenerateSubscriptionInvoiceCommand(subscriptionId));
        return Ok(invoice);
    }
}
