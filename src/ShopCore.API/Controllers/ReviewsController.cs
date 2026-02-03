using ShopCore.Api.Files;
using ShopCore.Application.Reviews.Commands.CreateReview;
using ShopCore.Application.Reviews.Commands.DeleteReview;
using ShopCore.Application.Reviews.Commands.MarkReviewHelpful;
using ShopCore.Application.Reviews.Commands.RespondToReview;
using ShopCore.Application.Reviews.Commands.UpdateReview;
using ShopCore.Application.Reviews.DTOs;
using ShopCore.Application.Reviews.Queries.GetMyReviews;
using ShopCore.Application.Reviews.Queries.GetProductReviews;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // Public endpoints
    // ----------------

    // GET /api/v1/reviews/products/{productId}/reviews
    [AllowAnonymous]
    [HttpGet("products/{productId:int}/reviews")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetProductReviews(
        int productId,
        [FromQuery] GetProductReviewsQuery query)
    {
        var finalQuery = query with { ProductId = productId };

        var reviews = await _mediator.Send(finalQuery);
        return Ok(reviews);
    }

    // ----------------
    // User actions
    // ----------------

    // POST /api/v1/reviews
    [Authorize]
    [HttpPost]
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

        var command = new CreateReviewCommand(
            productId,
            orderId,
            rating,
            title,
            comment,
            files);

        var review = await _mediator.Send(command);
        return Ok(review);
    }

    // PUT /api/v1/reviews/{id}
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(
        int id,
        [FromBody] UpdateReviewCommand command)
    {
        var finalCommand = command with { Id = id };

        var review = await _mediator.Send(finalCommand);
        return Ok(review);
    }

    // DELETE /api/v1/reviews/{id}
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _mediator.Send(new DeleteReviewCommand(id));
        return NoContent();
    }

    // GET /api/v1/reviews/me
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetMyReviews(
        [FromQuery] GetMyReviewsQuery query)
    {
        var reviews = await _mediator.Send(query);
        return Ok(reviews);
    }

    // POST /api/v1/reviews/{id}/helpful
    [Authorize]
    [HttpPost("{id:int}/helpful")]
    public async Task<IActionResult> MarkReviewHelpful(int id)
    {
        await _mediator.Send(new MarkReviewHelpfulCommand(id));
        return NoContent();
    }

    // ----------------
    // Vendor actions
    // ----------------

    // POST /api/v1/reviews/{id}/respond
    [Authorize(Roles = "Vendor")]
    [HttpPost("{id:int}/respond")]
    public async Task<IActionResult> AddVendorResponse(
        int id,
        [FromBody] RespondToReviewCommand command)
    {
        var finalCommand = command with { ReviewId = id };

        await _mediator.Send(finalCommand);
        return NoContent();
    }
}
