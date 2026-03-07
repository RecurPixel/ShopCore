using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;
using ShopCore.Application.Products.Queries.GetFeaturedProducts;
using ShopCore.Application.Products.Queries.GetProductById;
using ShopCore.Application.Products.Queries.GetProducts;
using ShopCore.Application.Products.Queries.SearchProducts;
using ShopCore.Application.Reviews.DTOs;
using ShopCore.Application.Reviews.Queries.GetProductReviews;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Public product catalog endpoints.
/// Vendor product management is at /api/v1/vendors/me/products
/// </summary>
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves products.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetProducts(
        [FromQuery] GetProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Searches for products.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> SearchProducts(
        [FromQuery] SearchProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves featured products.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>List&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("featured")]
    public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts(
        [FromQuery] GetFeaturedProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves product.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>ProductDetailDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetProductById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product is null)
            return NotFound();
        return Ok(product);
    }

    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetProductReviews(
        int id,
        [FromQuery] string? SortBy = null,
        [FromQuery] int Page = 1,
        [FromQuery] int PageSize = 20)
    {
        var reviews = await _mediator.Send(new GetProductReviewsQuery(id, SortBy, Page, PageSize));
        return Ok(reviews);
    }
}
