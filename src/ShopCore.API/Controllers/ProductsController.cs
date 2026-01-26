using ShopCore.Api.Files;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.Commands.CreateProduct;
using ShopCore.Application.Products.Commands.DeleteProduct;
using ShopCore.Application.Products.Commands.DeleteProductImage;
using ShopCore.Application.Products.Commands.UpdateProduct;
using ShopCore.Application.Products.Commands.UpdateProductStatus;
using ShopCore.Application.Products.Commands.UploadProductImages;
using ShopCore.Application.Products.DTOs;
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
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts(
        [FromQuery] GetProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // GET /api/v1/products/search
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> SearchProducts(
        [FromQuery] SearchProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // GET /api/v1/products/featured
    [HttpGet("featured")]
    public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts(
        [FromQuery] GetFeaturedProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // GET /api/v1/products/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetProductById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    // -------------------
    // Vendor-only actions
    // -------------------

    // POST /api/v1/products
    [Authorize(Roles = "Vendor")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    // PUT /api/v1/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        int id,
        [FromBody] UpdateProductCommand command)
    {
        var finalCommand = command with { Id = id };

        var product = await _mediator.Send(finalCommand);
        return Ok(product);
    }

    // DELETE /api/v1/products/{id}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return NoContent();
    }

    // POST /api/v1/products/{id}/images
    [Authorize(Roles = "Vendor")]
    [HttpPost("{id:int}/images")]
    public async Task<ActionResult<List<ProductImageDto>>> UploadProductImages(
        int id,
        List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest("At least one image is required.");

        var fileAdapters = files
            .Select(f => (IFile)new FormFileAdapter(f))
            .ToList();

        var command = new UploadProductImagesCommand(id, fileAdapters);

        var images = await _mediator.Send(command);
        return Ok(images);
    }

    // DELETE /api/v1/products/{id}/images/{imageId}
    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteProductImage(
        int id,
        int imageId)
    {
        await _mediator.Send(
            new DeleteProductImageCommand(id, imageId));

        return NoContent();
    }

    // PATCH /api/v1/products/{id}/status
    [Authorize(Roles = "Vendor")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateProductStatus(
        int id,
        [FromBody] UpdateProductStatusCommand command)
    {
        var finalCommand = command with { Id = id };

        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // POST /api/v1/products/{id}/specifications
    [Authorize(Roles = "Vendor")]
    [HttpPost("{id:int}/specifications")]
    public async Task<IActionResult> AddProductSpecification(
        int id,
        [FromBody] AddProductSpecificationCommand command)
    {
        var finalCommand = command with { ProductId = id };

        await _mediator.Send(finalCommand);
        return NoContent();
    }
}
