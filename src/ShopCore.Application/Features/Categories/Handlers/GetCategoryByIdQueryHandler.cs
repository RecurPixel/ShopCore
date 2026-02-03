using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
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
            .FirstOrDefaultAsync(cancellationToken);

        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);

        return category;
    }
}