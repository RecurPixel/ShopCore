using ShopCore.Application.Reviews.Commands.CreateReview;
using ShopCore.Application.Reviews.Commands.DeleteReview;
using ShopCore.Application.Reviews.Commands.MarkReviewHelpful;
using ShopCore.Application.Reviews.Commands.RespondToReview;
using ShopCore.Application.Reviews.Commands.UpdateReview;
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

    // GET /api/v1/reviews/products/{id}/reviews
    [HttpGet("products/{id}/reviews")]
    public async Task<IActionResult> GetProductReviews(
        Guid id,
        [FromQuery] GetProductReviewsQuery query
    )
    {
        query.ProductId = id;

        var reviews = await _mediator.Send(query);
        return Ok(reviews);
    }

    // ----------------
    // User actions
    // ----------------

    // POST /api/v1/reviews
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
    {
        var review = await _mediator.Send(command);
        return Ok(review);
    }

    // PUT /api/v1/reviews/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewCommand command)
    {
        command.ReviewId = id;

        var review = await _mediator.Send(command);
        return Ok(review);
    }

    // DELETE /api/v1/reviews/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        await _mediator.Send(new DeleteReviewCommand(id));
        return NoContent();
    }

    // POST /api/v1/reviews/{id}/helpful
    [Authorize]
    [HttpPost("{id}/helpful")]
    public async Task<IActionResult> MarkReviewHelpful(Guid id)
    {
        await _mediator.Send(new MarkReviewHelpfulCommand(id));
        return NoContent();
    }

    // ----------------
    // Vendor actions
    // ----------------

    // POST /api/v1/reviews/{id}/respond
    [Authorize(Roles = "Vendor")]
    [HttpPost("{id}/respond")]
    public async Task<IActionResult> RespondToReview(
        Guid id,
        [FromBody] RespondToReviewCommand command
    )
    {
        command.ReviewId = id;

        await _mediator.Send(command);
        return NoContent();
    }
}
