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

    // GET /api/v1/users/me
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var user = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(user);
    }

    // PUT /api/v1/users/me
    [HttpPut]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UpdateCurrentUserCommand command)
    {
        var user = await _mediator.Send(command);
        return Ok(user);
    }

    // POST /api/v1/users/me/avatar
    [HttpPost("avatar")]
    public async Task<ActionResult<string>> UploadAvatar(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Avatar file is required.");

        var command = new UploadUserAvatarCommand(new FormFileAdapter(file));
        var url = await _mediator.Send(command);
        return Ok(url);
    }

    // POST /api/v1/users/me/change-password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE /api/v1/users/me
    [HttpDelete]
    public async Task<IActionResult> DeleteMyAccount()
    {
        await _mediator.Send(new DeleteMyAccountCommand());
        return NoContent();
    }

    // ==================
    // Addresses
    // ==================

    // GET /api/v1/users/me/addresses
    [HttpGet("addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetMyAddresses()
    {
        var addresses = await _mediator.Send(new GetMyAddressesQuery());
        return Ok(addresses);
    }

    // POST /api/v1/users/me/addresses
    [HttpPost("addresses")]
    public async Task<ActionResult<AddressDto>> CreateAddress(
        [FromBody] CreateAddressCommand command)
    {
        var address = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
    }

    // GET /api/v1/users/me/addresses/{id}
    [HttpGet("addresses/{id:int}")]
    public async Task<ActionResult<AddressDto>> GetAddressById(int id)
    {
        var address = await _mediator.Send(new GetAddressByIdQuery(id));
        if (address is null)
            return NotFound();
        return Ok(address);
    }

    // PUT /api/v1/users/me/addresses/{id}
    [HttpPut("addresses/{id:int}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(
        int id,
        [FromBody] UpdateAddressCommand command)
    {
        var finalCommand = command with { Id = id };
        var address = await _mediator.Send(finalCommand);
        return Ok(address);
    }

    // DELETE /api/v1/users/me/addresses/{id}
    [HttpDelete("addresses/{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/users/me/addresses/{id}/default
    [HttpPatch("addresses/{id:int}/default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        await _mediator.Send(new SetDefaultAddressCommand(id));
        return NoContent();
    }

    // ==================
    // Orders (Customer view)
    // ==================

    // GET /api/v1/users/me/orders
    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetMyOrders(
        [FromQuery] GetMyOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/users/me/orders/{id}
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    // POST /api/v1/users/me/orders/{id}/cancel
    [HttpPost("orders/{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(
        int id,
        [FromBody] CancelOrderCommand command)
    {
        var finalCommand = command with { OrderId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // POST /api/v1/users/me/orders/items/{itemId}/cancel
    [HttpPost("orders/items/{itemId:int}/cancel")]
    public async Task<ActionResult<CancellationResultDto>> CancelOrderItem(
        int itemId,
        [FromBody] CancelOrderItemCommand command)
    {
        var finalCommand = command with { OrderItemId = itemId };
        var result = await _mediator.Send(finalCommand);
        return Ok(result);
    }

    // GET /api/v1/users/me/orders/{id}/invoice
    [HttpGet("orders/{id:int}/invoice")]
    public async Task<IActionResult> GetOrderInvoice(int id)
    {
        var invoiceBytes = await _mediator.Send(new GetOrderInvoiceQuery(id));
        if (invoiceBytes.FileContent is null || invoiceBytes.Length == 0)
            return NotFound();
        return File(invoiceBytes.FileContent, "application/pdf", $"invoice-order-{id}.pdf");
    }

    // ==================
    // Subscriptions (Customer view)
    // ==================

    // GET /api/v1/users/me/subscriptions
    [HttpGet("subscriptions")]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetMySubscriptions(
        [FromQuery] GetMySubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    // GET /api/v1/users/me/subscriptions/{id}
    [HttpGet("subscriptions/{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(id));
        return Ok(subscription);
    }

    // GET /api/v1/users/me/subscriptions/{id}/deliveries
    [HttpGet("subscriptions/{id:int}/deliveries")]
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetSubscriptionDeliveries(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var deliveries = await _mediator.Send(new GetSubscriptionDeliveriesQuery(id, page, pageSize));
        return Ok(deliveries);
    }

    // GET /api/v1/users/me/subscriptions/{id}/invoices
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

    // GET /api/v1/users/me/reviews
    [HttpGet("reviews")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetMyReviews(
        [FromQuery] GetMyReviewsQuery query)
    {
        var reviews = await _mediator.Send(query);
        return Ok(reviews);
    }

    // POST /api/v1/users/me/reviews
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

    // PUT /api/v1/users/me/reviews/{id}
    [HttpPut("reviews/{id:int}")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(
        int id,
        [FromBody] UpdateReviewCommand command)
    {
        var finalCommand = command with { Id = id };
        var review = await _mediator.Send(finalCommand);
        return Ok(review);
    }

    // DELETE /api/v1/users/me/reviews/{id}
    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _mediator.Send(new DeleteReviewCommand(id));
        return NoContent();
    }

    // ==================
    // Wishlist
    // ==================

    // GET /api/v1/users/me/wishlist
    [HttpGet("wishlist")]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    // POST /api/v1/users/me/wishlist
    [HttpPost("wishlist")]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE /api/v1/users/me/wishlist/{productId}
    [HttpDelete("wishlist/{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        await _mediator.Send(new RemoveFromWishlistCommand(productId));
        return NoContent();
    }

    // ==================
    // Payments
    // ==================

    // GET /api/v1/users/me/payments
    [HttpGet("payments")]
    public async Task<ActionResult<PaginatedList<PaymentHistoryDto>>> GetPaymentHistory(
        [FromQuery] GetPaymentHistoryQuery query)
    {
        var history = await _mediator.Send(query);
        return Ok(history);
    }
}
