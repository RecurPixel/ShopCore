using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
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

        return categories;
    }
}