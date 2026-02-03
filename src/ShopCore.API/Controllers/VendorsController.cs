using ShopCore.Api.Files;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Application.CustomerInvitations.Commands.CancelInvitation;
using ShopCore.Application.CustomerInvitations.Commands.CreateCustomerInvitation;
using ShopCore.Application.CustomerInvitations.Commands.ResendInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;
using ShopCore.Application.CustomerInvitations.Queries.GetCustomerInvitationById;
using ShopCore.Application.CustomerInvitations.Queries.GetMyCustomerInvitations;
using ShopCore.Application.Deliveries.Commands.CompleteDelivery;
using ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;
using ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;
using ShopCore.Application.Deliveries.DTOs;
using ShopCore.Application.Deliveries.Queries.GetDeliveryById;
using ShopCore.Application.Deliveries.Queries.GetVendorDeliveries;
using ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;
using ShopCore.Application.Invoices.DTOs;
using ShopCore.Application.Invoices.Queries.GetInvoiceById;
using ShopCore.Application.Invoices.Queries.GetVendorInvoices;
using ShopCore.Application.Orders.Commands.UpdateOrderItemStatus;
using ShopCore.Application.Orders.DTOs;
using ShopCore.Application.Payouts.DTOs;
using ShopCore.Application.Payouts.Queries.GetMyPayouts;
using ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;
using ShopCore.Application.Products.Commands.CreateProduct;
using ShopCore.Application.Products.Commands.DeleteProduct;
using ShopCore.Application.Products.Commands.DeleteProductImage;
using ShopCore.Application.Products.Commands.UpdateProduct;
using ShopCore.Application.Products.Commands.UpdateProductStatus;
using ShopCore.Application.Products.Commands.UploadProductImages;
using ShopCore.Application.Products.DTOs;
using ShopCore.Application.Products.Queries.GetProductById;
using ShopCore.Application.Subscriptions.DTOs;
using ShopCore.Application.Subscriptions.Queries.GetSubscriptionCustomerInfo;
using ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptionById;
using ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;
using ShopCore.Application.Vendors.Commands.RegisterVendor;
using ShopCore.Application.Vendors.Commands.UpdateMyVendor;
using ShopCore.Application.Vendors.Commands.UploadVendorLogo;
using ShopCore.Application.Vendors.DTOs;
using ShopCore.Application.Vendors.Queries.GetMyVendor;
using ShopCore.Application.Vendors.Queries.GetMyVendorOrders;
using ShopCore.Application.Vendors.Queries.GetMyVendorStats;
using ShopCore.Application.Vendors.Queries.GetVendorById;
using ShopCore.Application.Vendors.Queries.GetVendorCustomerById;
using ShopCore.Application.Vendors.Queries.GetVendorCustomerDeliveries;
using ShopCore.Application.Vendors.Queries.GetVendorCustomerOrders;
using ShopCore.Application.Vendors.Queries.GetVendorCustomers;
using ShopCore.Application.Vendors.Queries.GetVendorCustomerSubscriptions;
using ShopCore.Application.Vendors.Queries.GetVendorOrderItems;
using ShopCore.Application.Vendors.Queries.GetVendorProducts;
using ShopCore.Application.Vendors.Queries.SearchVendorsByLocation;
using ShopCore.Application.VendorServiceAreas.Commands.AddVendorServiceArea;
using ShopCore.Application.VendorServiceAreas.Commands.RemoveVendorServiceArea;
using ShopCore.Application.VendorServiceAreas.Commands.UpdateVendorServiceArea;
using ShopCore.Application.VendorServiceAreas.DTOs;
using ShopCore.Application.VendorServiceAreas.Queries.GetVendorServiceAreas;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/vendors")]
public class VendorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ==================
    // Public Endpoints
    // ==================

    // GET /api/v1/vendors/search
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<List<VendorSearchResultDto>>> SearchVendorsByLocation(
        [FromQuery] SearchVendorsByLocationQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    // GET /api/v1/vendors/{id}
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VendorPublicProfileDto>> GetVendorById(int id)
    {
        var vendor = await _mediator.Send(new GetVendorByIdQuery(id));
        if (vendor is null)
            return NotFound();
        return Ok(vendor);
    }

    // GET /api/v1/vendors/{id}/products
    [AllowAnonymous]
    [HttpGet("{id:int}/products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetVendorPublicProducts(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var products = await _mediator.Send(new GetVendorProductsQuery(id, page, pageSize, publicOnly: true));
        return Ok(products);
    }

    // ==================
    // Vendor Registration
    // ==================

    // POST /api/v1/vendors/register
    [Authorize]
    [HttpPost("register")]
    public async Task<ActionResult<VendorProfileDto>> RegisterVendor(
        [FromBody] RegisterVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // ==================
    // Vendor Profile (/me)
    // ==================

    // GET /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpGet("me")]
    public async Task<ActionResult<VendorProfileDto>> GetMyVendorProfile()
    {
        var vendor = await _mediator.Send(new GetMyVendorQuery());
        return Ok(vendor);
    }

    // PUT /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpPut("me")]
    public async Task<ActionResult<VendorProfileDto>> UpdateVendorProfile(
        [FromBody] UpdateMyVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // POST /api/v1/vendors/me/logo
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/logo")]
    public async Task<ActionResult<string>> UploadVendorLogo(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Logo file is required.");

        var command = new UploadVendorLogoCommand(new FormFileAdapter(file));
        var logoUrl = await _mediator.Send(command);
        return Ok(logoUrl);
    }

    // GET /api/v1/vendors/me/stats
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/stats")]
    public async Task<ActionResult<VendorStatsDto>> GetVendorStats()
    {
        var stats = await _mediator.Send(new GetMyVendorStatsQuery());
        return Ok(stats);
    }

    // ==================
    // Service Areas
    // ==================

    // GET /api/v1/vendors/me/service-areas
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/service-areas")]
    public async Task<ActionResult<List<VendorServiceAreaDto>>> GetMyServiceAreas()
    {
        var areas = await _mediator.Send(new GetVendorServiceAreasQuery());
        return Ok(areas);
    }

    // POST /api/v1/vendors/me/service-areas
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/service-areas")]
    public async Task<ActionResult<VendorServiceAreaDto>> AddServiceArea(
        [FromBody] AddVendorServiceAreaCommand command)
    {
        var area = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyServiceAreas), area);
    }

    // PUT /api/v1/vendors/me/service-areas/{id}
    [Authorize(Roles = "Vendor")]
    [HttpPut("me/service-areas/{id:int}")]
    public async Task<ActionResult<VendorServiceAreaDto>> UpdateServiceArea(
        int id,
        [FromBody] UpdateVendorServiceAreaCommand command)
    {
        var finalCommand = command with { ServiceAreaId = id };
        var area = await _mediator.Send(finalCommand);
        return Ok(area);
    }

    // DELETE /api/v1/vendors/me/service-areas/{id}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("me/service-areas/{id:int}")]
    public async Task<IActionResult> RemoveServiceArea(int id)
    {
        await _mediator.Send(new RemoveVendorServiceAreaCommand(id));
        return NoContent();
    }

    // ==================
    // Customer Invitations
    // ==================

    // GET /api/v1/vendors/me/invitations
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invitations")]
    public async Task<ActionResult<PaginatedList<CustomerInvitationDto>>> GetMyInvitations(
        [FromQuery] GetMyCustomerInvitationsQuery query)
    {
        var invitations = await _mediator.Send(query);
        return Ok(invitations);
    }

    // POST /api/v1/vendors/me/invitations
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/invitations")]
    public async Task<ActionResult<CustomerInvitationDto>> CreateInvitation(
        [FromBody] CreateCustomerInvitationCommand command)
    {
        var invitation = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvitationById), new { id = invitation.Id }, invitation);
    }

    // GET /api/v1/vendors/me/invitations/{id}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invitations/{id:int}")]
    public async Task<ActionResult<CustomerInvitationDto>> GetInvitationById(int id)
    {
        var invitation = await _mediator.Send(new GetCustomerInvitationByIdQuery(id));
        if (invitation is null)
            return NotFound();
        return Ok(invitation);
    }

    // POST /api/v1/vendors/me/invitations/{id}/resend
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/invitations/{id:int}/resend")]
    public async Task<IActionResult> ResendInvitation(int id)
    {
        await _mediator.Send(new ResendInvitationCommand(id));
        return NoContent();
    }

    // DELETE /api/v1/vendors/me/invitations/{id}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("me/invitations/{id:int}")]
    public async Task<IActionResult> CancelInvitation(int id)
    {
        await _mediator.Send(new CancelInvitationCommand(id));
        return NoContent();
    }

    // ==================
    // Products
    // ==================

    // GET /api/v1/vendors/me/products
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetMyProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var products = await _mediator.Send(new GetVendorProductsQuery(VendorId: null, page, pageSize, publicOnly: false));
        return Ok(products);
    }

    // POST /api/v1/vendors/me/products
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/products")]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyProductById), new { id = product.Id }, product);
    }

    // GET /api/v1/vendors/me/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/products/{id:int}")]
    public async Task<ActionResult<ProductDto>> GetMyProductById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null)
            return NotFound();
        return Ok(product);
    }

    // PUT /api/v1/vendors/me/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpPut("me/products/{id:int}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        int id,
        [FromBody] UpdateProductCommand command)
    {
        var finalCommand = command with { Id = id };
        var product = await _mediator.Send(finalCommand);
        return Ok(product);
    }

    // DELETE /api/v1/vendors/me/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("me/products/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/vendors/me/products/{id}/status
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/products/{id:int}/status")]
    public async Task<IActionResult> UpdateProductStatus(
        int id,
        [FromBody] UpdateProductStatusCommand command)
    {
        var finalCommand = command with { Id = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // POST /api/v1/vendors/me/products/{id}/images
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/products/{id:int}/images")]
    public async Task<ActionResult<List<string>>> UploadProductImages(
        int id,
        List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest("At least one image file is required.");

        var adaptedFiles = files.Select(f => (IFile)new FormFileAdapter(f)).ToList();
        var command = new UploadProductImagesCommand(id, adaptedFiles);
        var imageUrls = await _mediator.Send(command);
        return Ok(imageUrls);
    }

    // DELETE /api/v1/vendors/me/products/{id}/images/{imageId}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("me/products/{id:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteProductImage(int id, int imageId)
    {
        await _mediator.Send(new DeleteProductImageCommand(id, imageId));
        return NoContent();
    }

    // ==================
    // Orders (with vendor's products)
    // ==================

    // GET /api/v1/vendors/me/orders
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders")]
    public async Task<ActionResult<PaginatedList<VendorOrderDto>>> GetVendorOrders(
        [FromQuery] GetMyVendorOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/vendors/me/orders/{orderId}/items
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders/{orderId:int}/items")]
    public async Task<ActionResult<List<VendorOrderItemDto>>> GetVendorOrderItems(int orderId)
    {
        var items = await _mediator.Send(new GetVendorOrderItemsQuery(orderId));
        return Ok(items);
    }

    // PATCH /api/v1/vendors/me/orders/items/{itemId}/status
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/orders/items/{itemId:int}/status")]
    public async Task<IActionResult> UpdateOrderItemStatus(
        int itemId,
        [FromBody] UpdateOrderItemStatusCommand command)
    {
        var finalCommand = command with { OrderItemId = itemId };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // ==================
    // Subscriptions (for vendor's products)
    // ==================

    // GET /api/v1/vendors/me/subscriptions
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/subscriptions")]
    public async Task<ActionResult<PaginatedList<VendorSubscriptionDto>>> GetVendorSubscriptions(
        [FromQuery] GetVendorSubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    // GET /api/v1/vendors/me/subscriptions/{id}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/subscriptions/{id:int}")]
    public async Task<ActionResult<VendorSubscriptionDto>> GetVendorSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetVendorSubscriptionByIdQuery(id));
        if (subscription is null)
            return NotFound();
        return Ok(subscription);
    }

    // GET /api/v1/vendors/me/subscriptions/{id}/customer
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/subscriptions/{id:int}/customer")]
    public async Task<ActionResult<SubscriptionCustomerInfoDto>> GetSubscriptionCustomerInfo(int id)
    {
        var customerInfo = await _mediator.Send(new GetSubscriptionCustomerInfoQuery(id));
        if (customerInfo is null)
            return NotFound();
        return Ok(customerInfo);
    }

    // ==================
    // Deliveries
    // ==================

    // GET /api/v1/vendors/me/deliveries
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetVendorDeliveries(
        [FromQuery] GetVendorDeliveriesQuery query)
    {
        var deliveries = await _mediator.Send(query);
        return Ok(deliveries);
    }

    // GET /api/v1/vendors/me/deliveries/{id}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/deliveries/{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetDeliveryById(int id)
    {
        var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));
        if (delivery is null)
            return NotFound();
        return Ok(delivery);
    }

    // PATCH /api/v1/vendors/me/deliveries/{id}/complete
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/deliveries/{id:int}/complete")]
    public async Task<IActionResult> CompleteDelivery(
        int id,
        [FromForm] IFormFile? deliveryPhoto)
    {
        IFile? photo = deliveryPhoto is not null ? new FormFileAdapter(deliveryPhoto) : null;
        await _mediator.Send(new CompleteDeliveryCommand(id, photo));
        return NoContent();
    }

    // PATCH /api/v1/vendors/me/deliveries/{id}/failed
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/deliveries/{id:int}/failed")]
    public async Task<IActionResult> MarkDeliveryFailed(
        int id,
        [FromBody] MarkDeliveryFailedCommand command)
    {
        var finalCommand = command with { DeliveryId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // POST /api/v1/vendors/me/deliveries/{id}/photo
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/deliveries/{id:int}/photo")]
    public async Task<ActionResult<string>> UploadDeliveryPhoto(int id, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Photo file is required.");

        var command = new UploadDeliveryPhotoCommand(id, new FormFileAdapter(file));
        var photoUrl = await _mediator.Send(command);
        return Ok(photoUrl);
    }

    // ==================
    // Invoices
    // ==================

    // GET /api/v1/vendors/me/invoices
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invoices")]
    public async Task<ActionResult<PaginatedList<InvoiceDto>>> GetVendorInvoices(
        [FromQuery] GetVendorInvoicesQuery query)
    {
        var invoices = await _mediator.Send(query);
        return Ok(invoices);
    }

    // GET /api/v1/vendors/me/invoices/{id}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invoices/{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (invoice is null)
            return NotFound();
        return Ok(invoice);
    }

    // POST /api/v1/vendors/me/subscriptions/{id}/invoices
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/subscriptions/{id:int}/invoices")]
    public async Task<ActionResult<InvoiceDto>> GenerateSubscriptionInvoice(int id)
    {
        var invoice = await _mediator.Send(new GenerateSubscriptionInvoiceCommand(id));
        return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
    }

    // ==================
    // Payouts
    // ==================

    // GET /api/v1/vendors/me/payouts
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/payouts")]
    public async Task<ActionResult<PaginatedList<PayoutDto>>> GetMyPayouts(
        [FromQuery] GetMyPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    // GET /api/v1/vendors/me/payouts/pending
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/payouts/pending")]
    public async Task<ActionResult<PendingPayoutDto>> GetPendingPayoutAmount()
    {
        var pending = await _mediator.Send(new GetPendingPayoutAmountQuery());
        return Ok(pending);
    }

    // ==================
    // Customers (Vendor's view)
    // ==================

    // GET /api/v1/vendors/me/customers
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers")]
    public async Task<ActionResult<PaginatedList<VendorCustomerDto>>> GetVendorCustomers(
        [FromQuery] GetVendorCustomersQuery query)
    {
        var customers = await _mediator.Send(query);
        return Ok(customers);
    }

    // GET /api/v1/vendors/me/customers/{userId}
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}")]
    public async Task<ActionResult<VendorCustomerDetailDto>> GetVendorCustomerById(int userId)
    {
        var customer = await _mediator.Send(new GetVendorCustomerByIdQuery(userId));
        if (customer is null)
            return NotFound();
        return Ok(customer);
    }

    // GET /api/v1/vendors/me/customers/{userId}/subscriptions
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/subscriptions")]
    public async Task<ActionResult<PaginatedList<VendorSubscriptionDto>>> GetVendorCustomerSubscriptions(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var subscriptions = await _mediator.Send(new GetVendorCustomerSubscriptionsQuery(userId, page, pageSize));
        return Ok(subscriptions);
    }

    // GET /api/v1/vendors/me/customers/{userId}/orders
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/orders")]
    public async Task<ActionResult<PaginatedList<VendorOrderDto>>> GetVendorCustomerOrders(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var orders = await _mediator.Send(new GetVendorCustomerOrdersQuery(userId, page, pageSize));
        return Ok(orders);
    }

    // GET /api/v1/vendors/me/customers/{userId}/deliveries
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetVendorCustomerDeliveries(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var deliveries = await _mediator.Send(new GetVendorCustomerDeliveriesQuery(userId, page, pageSize));
        return Ok(deliveries);
    }
}
