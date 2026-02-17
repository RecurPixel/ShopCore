using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedList<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories)
            .AsQueryable();

        // Apply filters
        if (request.ParentId.HasValue)
            query = query.Where(c => c.ParentCategoryId == request.ParentId);
        else
            query = query.Where(c => c.ParentCategoryId == null);

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(c => c.Name.Contains(request.Search));

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentCategoryId = c.ParentCategoryId,
                DisplayOrder = c.DisplayOrder,
                ProductCount = c.Products.Count(p => p.Status == ProductStatus.Active),
                SubCategories = c.SubCategories
                    .OrderBy(sc => sc.DisplayOrder)
                    .Select(sc => new CategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        Slug = sc.Slug,
                        Description = sc.Description,
                        ImageUrl = sc.ImageUrl,
                        ParentCategoryId = sc.ParentCategoryId,
                        DisplayOrder = sc.DisplayOrder,
                        ProductCount = sc.Products.Count(p => p.Status == ProductStatus.Active)
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<CategoryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}