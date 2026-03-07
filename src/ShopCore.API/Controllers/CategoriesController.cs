using ShopCore.Api.Files;
using ShopCore.Application.Categories.Commands.CreateCategory;
using ShopCore.Application.Categories.Commands.DeleteCategory;
using ShopCore.Application.Categories.Commands.UpdateCategory;
using ShopCore.Application.Categories.Commands.UploadCategoryImage;
using ShopCore.Application.Categories.DTOs;
using ShopCore.Application.Categories.Queries.GetCategories;
using ShopCore.Application.Categories.Queries.GetCategoryById;
using ShopCore.Application.Categories.Queries.GetProductsByCategory;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
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

    /// <summary>
    /// Retrieves categories.
    /// </summary>
    /// <returns>List&lt;CategoryDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves category.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>CategoryDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _mediator.Send(
            new GetCategoryByIdQuery(id));

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Retrieves products by category.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("{id:int}/products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetProductsByCategory(
        int id,
        [FromQuery] GetProductsByCategoryQuery query)
    {
        var finalQuery = query with { CategoryId = id };

        var products = await _mediator.Send(finalQuery);
        return Ok(products);
    }

    // -------------
    // Admin actions
    // -------------

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CategoryDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates category.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>CategoryDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        int id,
        [FromBody] UpdateCategoryCommand command)
    {
        var finalCommand = command with { Id = id };

        var category = await _mediator.Send(finalCommand);
        return Ok(category);
    }

    /// <summary>
    /// Deletes category.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Uploads category image.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="file">The file to upload</param>
    /// <returns>string</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/image")]
    public async Task<ActionResult<string>> UploadCategoryImage(
    int id,
    IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Image file is required.");

        var command = new UploadCategoryImageCommand(
            id,
            new FormFileAdapter(file));

        var imageUrl = await _mediator.Send(command);
        return Ok(imageUrl);
    }
}
