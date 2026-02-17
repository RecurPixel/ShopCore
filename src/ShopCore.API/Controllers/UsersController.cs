using ShopCore.Api.Files;
using ShopCore.Application.Addresses.Commands.CreateAddress;
using ShopCore.Application.Addresses.Commands.DeleteAddress;
using ShopCore.Application.Addresses.Commands.SetDefaultAddress;
using ShopCore.Application.Addresses.Commands.UpdateAddress;
using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Addresses.Queries.GetAddressById;
using ShopCore.Application.Addresses.Queries.GetMyAddresses;
using ShopCore.Application.Deliveries.DTOs;
using ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;
using ShopCore.Application.Invoices.DTOs;
using ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;
using ShopCore.Application.Orders.Commands.CancelOrder;
using ShopCore.Application.Orders.Commands.CancelOrderItem;
using ShopCore.Application.Orders.DTOs;
using ShopCore.Application.Orders.Queries.GetMyOrders;
using ShopCore.Application.Orders.Queries.GetOrderById;
using ShopCore.Application.Orders.Queries.GetOrderInvoice;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Application.Payments.Queries.GetPaymentHistory;
using ShopCore.Application.Reviews.Commands.CreateReview;
using ShopCore.Application.Reviews.Commands.DeleteReview;
using ShopCore.Application.Reviews.Commands.UpdateReview;
using ShopCore.Application.Reviews.DTOs;
using ShopCore.Application.Reviews.Queries.GetMyReviews;
using ShopCore.Application.Subscriptions.DTOs;
using ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;
using ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;
using ShopCore.Application.Users.Commands.ChangePassword;
using ShopCore.Application.Users.Commands.DeleteMyAccount;
using ShopCore.Application.Users.Commands.UpdateCurrentUser;
using ShopCore.Application.Users.Commands.UploadUserAvatar;
using ShopCore.Application.Users.DTOs;
using ShopCore.Application.Users.Queries.GetCurrentUser;
using ShopCore.Application.Wishlist.Commands.AddToWishlist;
using ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;
using ShopCore.Application.Wishlist.DTOs;
using ShopCore.Application.Wishlist.Queries.GetWishlist;

namespace ShopCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users/me")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ==================
    // Profile
    // ==================

    /// <summary>
    /// Retrieves profile.
    /// </summary>
    /// <returns>UserProfileDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var user = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(user);
    }

    /// <summary>
    /// Updates profile.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>UserProfileDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UpdateCurrentUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    /// <summary>
    /// Uploads avatar.
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>string</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("avatar")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Avatar file is required.");

        var command = new UploadUserAvatarCommand(new FormFileAdapter(file));
        var url = await _mediator.Send(command);
        return Ok(url);
    }

    /// <summary>
    /// Creates or processes change password.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes my account.
    /// </summary>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete]
    public async Task<IActionResult> DeleteMyAccount()
    {
        await _mediator.Send(new DeleteMyAccountCommand());
        return NoContent();
    }

    // ==================
    // Addresses
    // ==================

    /// <summary>
    /// Retrieves my addresses.
    /// </summary>
    /// <returns>List&lt;AddressDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetMyAddresses()
    {
        var addresses = await _mediator.Send(new GetMyAddressesQuery());
        return Ok(addresses);
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>AddressDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("addresses")]
    public async Task<ActionResult<AddressDto>> CreateAddress(
        [FromBody] CreateAddressCommand command)
    {
        var address = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
    }

    /// <summary>
    /// Retrieves address.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>AddressDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("addresses/{id:int}")]
    public async Task<ActionResult<AddressDto>> GetAddressById(int id)
    {
        var address = await _mediator.Send(new GetAddressByIdQuery(id));
        if (address is null)
            return NotFound();
        return Ok(address);
    }

    /// <summary>
    /// Updates address.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>AddressDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPut("addresses/{id:int}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(
        int id,
        [FromBody] UpdateAddressCommand command)
    {
        var finalCommand = command with { Id = id };
        var address = await _mediator.Send(finalCommand);
        return Ok(address);
    }

    /// <summary>
    /// Deletes address.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("addresses/{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Sets default address.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPatch("addresses/{id:int}/default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        await _mediator.Send(new SetDefaultAddressCommand(id));
        return NoContent();
    }

    // ==================
    // Orders (Customer view)
    // ==================

    /// <summary>
    /// Retrieves my orders.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;OrderDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetMyOrders(
        [FromQuery] GetMyOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves order.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>OrderDetailDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    /// <summary>
    /// Cancels order.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("orders/{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(
        int id,
        [FromBody] CancelOrderCommand command)
    {
        var finalCommand = command with { OrderId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    /// <summary>
    /// Cancels order item.
    /// </summary>
    /// <param name="itemId">The item identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>CancellationResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("orders/items/{itemId:int}/cancel")]
    public async Task<ActionResult<CancellationResultDto>> CancelOrderItem(
        int itemId,
        [FromBody] CancelOrderItemCommand command)
    {
        var finalCommand = command with { OrderItemId = itemId };
        var result = await _mediator.Send(finalCommand);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves order invoice.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("orders/{id:int}/invoice")]
    public async Task<IActionResult> GetOrderInvoice(int id)
    {
        var invoiceBytes = await _mediator.Send(new GetOrderInvoiceQuery(id));
        if (invoiceBytes.Content is null || invoiceBytes.FileSize == 0)
            return NotFound();
        return File(invoiceBytes.Content, "application/pdf", $"invoice-order-{id}.pdf");
    }

    // ==================
    // Subscriptions (Customer view)
    // ==================

    /// <summary>
    /// Retrieves my subscriptions.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;SubscriptionDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("subscriptions")]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetMySubscriptions(
        [FromQuery] GetMySubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Retrieves subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>SubscriptionDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("subscriptions/{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(id));
        return Ok(subscription);
    }

    /// <summary>
    /// Retrieves subscription deliveries.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="page">Page number for pagination (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>PaginatedList&lt;DeliveryDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("subscriptions/{id:int}/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetSubscriptionDeliveries(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var deliveries = await _mediator.Send(new GetSubscriptionDeliveriesQuery(id, page, pageSize));
        return Ok(deliveries);
    }

    /// <summary>
    /// Retrieves subscription invoices.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;InvoiceDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("subscriptions/{id:int}/invoices")]
    public async Task<ActionResult<PaginatedList<InvoiceDto>>> GetSubscriptionInvoices(
        int id,
        [FromQuery] GetSubscriptionInvoicesQuery query)
    {
        var finalQuery = query with { SubscriptionId = id };
        var invoices = await _mediator.Send(finalQuery);
        return Ok(invoices);
    }

    // ==================
    // Reviews
    // ==================

    /// <summary>
    /// Retrieves my reviews.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;ReviewDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("reviews")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetMyReviews(
        [FromQuery] GetMyReviewsQuery query)
    {
        var reviews = await _mediator.Send(query);
        return Ok(reviews);
    }

    [HttpPost("reviews")]
    public async Task<ActionResult<ReviewDto>> CreateReview(
        [FromForm] int productId,
        [FromForm] int orderId,
        [FromForm] int rating,
        [FromForm] string? title,
        [FromForm] string? comment,
        [FromForm] List<IFormFile>? images)
    {
        var files = images?
            .Select(f => (IFile)new FormFileAdapter(f))
            .ToList();

        var command = new CreateReviewCommand(productId, orderId, rating, title, comment, files);
        var review = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyReviews), review);
    }

    /// <summary>
    /// Updates review.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>ReviewDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPut("reviews/{id:int}")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(
        int id,
        [FromBody] UpdateReviewCommand command)
    {
        var finalCommand = command with { Id = id };
        var review = await _mediator.Send(finalCommand);
        return Ok(review);
    }

    /// <summary>
    /// Deletes review.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _mediator.Send(new DeleteReviewCommand(id));
        return NoContent();
    }

    // ==================
    // Wishlist
    // ==================

    /// <summary>
    /// Retrieves wishlist.
    /// </summary>
    /// <returns>WishlistDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("wishlist")]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    /// <summary>
    /// Adds to wishlist.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("wishlist")]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes from wishlist.
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("wishlist/{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        await _mediator.Send(new RemoveFromWishlistCommand(productId));
        return NoContent();
    }

    // ==================
    // Payments
    // ==================

    /// <summary>
    /// Retrieves payment history.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;PaymentHistoryDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpGet("payments")]
    public async Task<ActionResult<PaginatedList<PaymentHistoryDto>>> GetPaymentHistory(
        [FromQuery] GetPaymentHistoryQuery query)
    {
        var history = await _mediator.Send(query);
        return Ok(history);
    }
}
