using ShopCore.Api.Files;
using ShopCore.Application.Categories.Commands.CreateCategory;
using ShopCore.Application.Categories.Commands.DeleteCategory;
using ShopCore.Application.Categories.Commands.UpdateCategory;
using ShopCore.Application.Categories.Commands.UploadCategoryImage;
using ShopCore.Application.Categories.DTOs;
using ShopCore.Application.Categories.Queries.GetCategories;
using ShopCore.Application.Categories.Queries.GetCategoryById;
using ShopCore.Application.Categories.Queries.GetProductsByCategory;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

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
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }

    // GET /api/v1/categories/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _mediator.Send(
            new GetCategoryByIdQuery(id));

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    // GET /api/v1/categories/{id}/products
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

    // POST /api/v1/categories
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CreateCategoryCommand command)
    {
        var category = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    // PUT /api/v1/categories/{id}
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

    // DELETE /api/v1/categories/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }

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
