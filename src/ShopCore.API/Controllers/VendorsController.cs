using ShopCore.Api.Files;
using ShopCore.Api.Models;
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

    /// <summary>
    /// Searches for vendors by location.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>List&lt;VendorSearchResultDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<List<VendorSearchResultDto>>> SearchVendorsByLocation(
        [FromQuery] SearchVendorsByLocationQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    /// <summary>
    /// Retrieves vendor.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>VendorPublicProfileDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VendorPublicProfileDto>> GetVendorById(int id)
    {
        var vendor = await _mediator.Send(new GetVendorByIdQuery(id));
        if (vendor is null)
            return NotFound();
        return Ok(vendor);
    }

    /// <summary>
    /// Retrieves vendor public products.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="Page">Page number for pagination (1-based)</param>
    /// <param name="PageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [AllowAnonymous]
    [HttpGet("{id:int}/products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetVendorPublicProducts(
        int id,
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var products = await _mediator.Send(new GetVendorProductsQuery(id, Page, PageSize, publicOnly: true));
        return Ok(products);
    }

    // ==================
    // Vendor Registration
    // ==================

    /// <summary>
    /// Registers a new vendor.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>VendorProfileDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves my vendor profile.
    /// </summary>
    /// <returns>VendorProfileDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me")]
    public async Task<ActionResult<VendorProfileDto>> GetMyVendorProfile()
    {
        var vendor = await _mediator.Send(new GetMyVendorQuery());
        return Ok(vendor);
    }

    /// <summary>
    /// Updates vendor profile.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>VendorProfileDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPut("me")]
    public async Task<ActionResult<VendorProfileDto>> UpdateVendorProfile(
        [FromBody] UpdateMyVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    /// <summary>
    /// Uploads vendor logo.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>string</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves vendor stats.
    /// </summary>
    /// <returns>VendorStatsDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
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

    /// <summary>
    /// Retrieves my service areas.
    /// </summary>
    /// <returns>List&lt;VendorServiceAreaDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/service-areas")]
    public async Task<ActionResult<List<VendorServiceAreaDto>>> GetMyServiceAreas()
    {
        var areas = await _mediator.Send(new GetVendorServiceAreasQuery());
        return Ok(areas);
    }

    /// <summary>
    /// Adds service area.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>VendorServiceAreaDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/service-areas")]
    public async Task<ActionResult<VendorServiceAreaDto>> AddServiceArea(
        [FromBody] AddVendorServiceAreaCommand command)
    {
        var area = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyServiceAreas), area);
    }

    /// <summary>
    /// Updates service area.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>VendorServiceAreaDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Removes service area.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves my invitations.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;CustomerInvitationDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invitations")]
    public async Task<ActionResult<PaginatedList<CustomerInvitationDto>>> GetMyInvitations(
        [FromQuery] GetMyCustomerInvitationsQuery query)
    {
        var invitations = await _mediator.Send(query);
        return Ok(invitations);
    }

    /// <summary>
    /// Creates a new invitation.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CustomerInvitationDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/invitations")]
    public async Task<ActionResult<CustomerInvitationDto>> CreateInvitation(
        [FromBody] CreateCustomerInvitationCommand command)
    {
        var invitation = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvitationById), new { id = invitation.Id }, invitation);
    }

    /// <summary>
    /// Retrieves invitation.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>CustomerInvitationDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invitations/{id:int}")]
    public async Task<ActionResult<CustomerInvitationDto>> GetInvitationById(int id)
    {
        var invitation = await _mediator.Send(new GetCustomerInvitationByIdQuery(id));
        if (invitation is null)
            return NotFound();
        return Ok(invitation);
    }

    /// <summary>
    /// Resends invitation.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/invitations/{id:int}/resend")]
    public async Task<IActionResult> ResendInvitation(int id)
    {
        await _mediator.Send(new ResendInvitationCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Cancels invitation.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves my products.
    /// </summary>
    /// <param name="Page">Page number for pagination (1-based)</param>
    /// <param name="PageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetMyProducts(
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var products = await _mediator.Send(new GetVendorProductsQuery(VendorId: null, Page, PageSize, publicOnly: false));
        return Ok(products);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>ProductDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/products")]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyProductById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Retrieves my product.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>ProductDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/products/{id:int}")]
    public async Task<ActionResult<ProductDto>> GetMyProductById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null)
            return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// Updates product.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>ProductDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Deletes product.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpDelete("me/products/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Updates product status.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Uploads product images.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="files">The files</param>
    /// <returns>List&lt;string&gt;</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Deletes product image.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="imageId">The image identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves vendor orders.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;VendorOrderDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders")]
    public async Task<ActionResult<PaginatedList<VendorOrderDto>>> GetVendorOrders(
        [FromQuery] GetMyVendorOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves vendor order items.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>List&lt;VendorOrderItemDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders/{orderId:int}/items")]
    public async Task<ActionResult<List<VendorOrderItemDto>>> GetVendorOrderItems(int orderId)
    {
        var items = await _mediator.Send(new GetVendorOrderItemsQuery(orderId));
        return Ok(items);
    }

    /// <summary>
    /// Updates order item status.
    /// </summary>
    /// <param name="itemId">The item identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves vendor subscriptions.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;VendorSubscriptionDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/subscriptions")]
    public async Task<ActionResult<PaginatedList<VendorSubscriptionDto>>> GetVendorSubscriptions(
        [FromQuery] GetVendorSubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Retrieves vendor subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>VendorSubscriptionDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/subscriptions/{id:int}")]
    public async Task<ActionResult<VendorSubscriptionDto>> GetVendorSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetVendorSubscriptionByIdQuery(id));
        if (subscription is null)
            return NotFound();
        return Ok(subscription);
    }

    /// <summary>
    /// Retrieves subscription customer info.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>SubscriptionCustomerInfoDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
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

    /// <summary>
    /// Retrieves vendor deliveries.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;DeliveryDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetVendorDeliveries(
        [FromQuery] GetVendorDeliveriesQuery query)
    {
        var deliveries = await _mediator.Send(query);
        return Ok(deliveries);
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
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/deliveries/{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetDeliveryById(int id)
    {
        var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));
        if (delivery is null)
            return NotFound();
        return Ok(delivery);
    }

    /// <summary>
    /// Completes delivery.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="request">Form field for request</param>
    /// <returns>DeliveryDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/deliveries/{id:int}/complete")]
    public async Task<ActionResult<DeliveryDto>> CompleteDelivery(
        int id,
        [FromForm] CompleteDeliveryRequest? request)
    {
        // Upload delivery photo if provided
        string? deliveryPhotoUrl = null;
        if (request?.DeliveryPhoto != null)
        {
            var photoCommand = new UploadDeliveryPhotoCommand(
                id,
                new FormFileAdapter(request.DeliveryPhoto));
            deliveryPhotoUrl = await _mediator.Send(photoCommand);
        }

        // Upload customer signature if provided
        string? signatureUrl = null;
        if (request?.CustomerSignature != null)
        {
            var signatureCommand = new UploadDeliveryPhotoCommand(
                id,
                new FormFileAdapter(request.CustomerSignature));
            signatureUrl = await _mediator.Send(signatureCommand);
        }

        // Parse item statuses
        List<DeliveryItemStatusInput>? itemStatuses = null;
        if (request?.ItemStatuses != null && request.ItemStatuses.Any())
        {
            itemStatuses = request.ItemStatuses
                .Select(i => new DeliveryItemStatusInput(
                    i.DeliveryItemId,
                    i.Status,
                    i.Notes))
                .ToList();
        }

        // Complete the delivery (PaymentMethod is determined by delivery record, not user input)
        var command = new CompleteDeliveryCommand(
            id,
            itemStatuses,
            request?.PaymentTransactionId,  // CollectedPaymentReference for COD
            deliveryPhotoUrl ?? request?.DeliveryPhotoUrl,
            signatureUrl ?? request?.CustomerSignatureUrl,
            request?.DeliveryNotes
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Marks delivery failed.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Uploads delivery photo.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="file">The file to upload</param>
    /// <returns>string</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves vendor invoices.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;InvoiceDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invoices")]
    public async Task<ActionResult<PaginatedList<InvoiceDto>>> GetVendorInvoices(
        [FromQuery] GetVendorInvoicesQuery query)
    {
        var invoices = await _mediator.Send(query);
        return Ok(invoices);
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
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/invoices/{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
    {
        var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (invoice is null)
            return NotFound();
        return Ok(invoice);
    }

    /// <summary>
    /// Creates or processes generate subscription invoice.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>InvoiceDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves my payouts.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;PayoutDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/payouts")]
    public async Task<ActionResult<PaginatedList<PayoutDto>>> GetMyPayouts(
        [FromQuery] GetMyPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    /// <summary>
    /// Retrieves pending payout amount.
    /// </summary>
    /// <returns>PendingPayoutDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves vendor customers.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;VendorCustomerDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers")]
    public async Task<ActionResult<PaginatedList<VendorCustomerDto>>> GetVendorCustomers(
        [FromQuery] GetVendorCustomersQuery query)
    {
        var customers = await _mediator.Send(query);
        return Ok(customers);
    }

    /// <summary>
    /// Retrieves vendor customer.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>VendorCustomerDetailDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}")]
    public async Task<ActionResult<VendorCustomerDetailDto>> GetVendorCustomerById(int userId)
    {
        var customer = await _mediator.Send(new GetVendorCustomerByIdQuery(userId));
        if (customer is null)
            return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Retrieves vendor customer subscriptions.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="Page">Page number for pagination (1-based)</param>
    /// <param name="PageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;VendorSubscriptionDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/subscriptions")]
    public async Task<ActionResult<PaginatedList<VendorSubscriptionDto>>> GetVendorCustomerSubscriptions(
        int userId,
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var subscriptions = await _mediator.Send(new GetVendorCustomerSubscriptionsQuery(userId, Page, PageSize));
        return Ok(subscriptions);
    }

    /// <summary>
    /// Retrieves vendor customer orders.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="Page">Page number for pagination (1-based)</param>
    /// <param name="PageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;VendorOrderDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/orders")]
    public async Task<ActionResult<PaginatedList<VendorOrderDto>>> GetVendorCustomerOrders(
        int userId,
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var orders = await _mediator.Send(new GetVendorCustomerOrdersQuery(userId, Page, PageSize));
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves vendor customer deliveries.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="Page">Page number for pagination (1-based)</param>
    /// <param name="PageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;DeliveryDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/customers/{userId:int}/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetVendorCustomerDeliveries(
        int userId,
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var deliveries = await _mediator.Send(new GetVendorCustomerDeliveriesQuery(userId, Page, PageSize));
        return Ok(deliveries);
    }
}
