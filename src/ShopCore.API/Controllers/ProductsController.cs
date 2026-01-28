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

    // GET /api/v1/products/{id}/reviews
    [HttpGet("{id:int}/reviews")]
    public async Task<ActionResult<PaginatedList<ReviewDto>>> GetProductReviews(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var reviews = await _mediator.Send(new GetProductReviewsQuery(id, page, pageSize));
        return Ok(reviews);
    }
}
