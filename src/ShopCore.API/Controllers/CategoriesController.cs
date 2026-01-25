namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // Public endpoints
    // ----------------

    // GET /api/v1/categories
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }

    // GET /api/v1/categories/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // GET /api/v1/categories/{id}/products
    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetCategoryProducts(Guid id)
    {
        var products = await _mediator.Send(new GetProductsByCategoryQuery(id));

        return Ok(products);
    }

    // -------------
    // Admin actions
    // -------------

    // POST /api/v1/categories
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    // PUT /api/v1/categories/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryCommand command
    )
    {
        command.CategoryId = id;

        var category = await _mediator.Send(command);
        return Ok(category);
    }

    // DELETE /api/v1/categories/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
