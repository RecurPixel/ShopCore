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

    // GET /api/v1/invoices/subscriptions/{id}/invoices
    [Authorize]
    [HttpGet("subscriptions/{id}/invoices")]
    public async Task<IActionResult> GetSubscriptionInvoices(Guid id)
    {
        var invoices = await _mediator.Send(new GetSubscriptionInvoicesQuery(id));

        return Ok(invoices);
    }

    // GET /api/v1/invoices/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    // POST /api/v1/invoices/{id}/pay
    [Authorize]
    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayInvoice(Guid id)
    {
        await _mediator.Send(new PayInvoiceCommand(id));
        return NoContent();
    }

    // GET /api/v1/invoices/{id}/download
    [Authorize]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadInvoice(Guid id)
    {
        var file = await _mediator.Send(new DownloadInvoiceQuery(id));

        return File(file.Content, file.ContentType, file.FileName);
    }

    // ----------------
    // Vendor endpoint
    // ----------------

    // POST /api/v1/invoices/subscriptions/{id}/generate
    [Authorize(Roles = "Vendor")]
    [HttpPost("subscriptions/{id}/generate")]
    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        await _mediator.Send(new GenerateSubscriptionInvoiceCommand(id));
        return NoContent();
    }
}
