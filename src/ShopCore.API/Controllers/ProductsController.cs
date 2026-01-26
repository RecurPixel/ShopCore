using ShopCore.Application.Products.Commands.CreateProduct;
using ShopCore.Application.Products.Commands.DeleteProduct;
using ShopCore.Application.Products.Commands.DeleteProductImage;
using ShopCore.Application.Products.Commands.UpdateProduct;
using ShopCore.Application.Products.Commands.UpdateProductStatus;
using ShopCore.Application.Products.Commands.UploadProductImages;
using ShopCore.Application.Products.Queries.GetFeaturedProducts;
using ShopCore.Application.Products.Queries.GetProductById;
using ShopCore.Application.Products.Queries.GetProducts;
using ShopCore.Application.Products.Queries.SearchProducts;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // Public endpoints
    // ----------------

    // GET /api/v1/products
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // GET /api/v1/products/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET /api/v1/products/featured
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts()
    {
        var products = await _mediator.Send(new GetFeaturedProductsQuery());
        return Ok(products);
    }

    // GET /api/v1/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // -------------------
    // Vendor-only actions
    // -------------------

    // POST /api/v1/products
    [Authorize(Roles = "Vendor")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    // PUT /api/v1/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.ProductId = id;

        var product = await _mediator.Send(command);
        return Ok(product);
    }

    // DELETE /api/v1/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }

    // POST /api/v1/products/{id}/images
    [Authorize(Roles = "Vendor")]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadProductImages(
        Guid id,
        [FromForm] UploadProductImagesCommand command
    )
    {
        command.ProductId = id;

        var images = await _mediator.Send(command);
        return Ok(images);
    }

    // DELETE /api/v1/products/{id}/images/{imageId}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteProductImage(Guid id, Guid imageId)
    {
        await _mediator.Send(new DeleteProductImageCommand(id, imageId));

        return NoContent();
    }

    // PATCH /api/v1/products/{id}/status
    [Authorize(Roles = "Vendor")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateProductStatus(
        Guid id,
        [FromBody] UpdateProductStatusCommand command
    )
    {
        command.ProductId = id;

        await _mediator.Send(command);
        return NoContent();
    }
}
