using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can create categories");

        var slug = await GenerateUniqueSlugAsync(request.Name, ct);

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(ct);

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ImageUrl,
            category.ParentCategoryId,
            null,
            category.DisplayOrder,
            true,
            0);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = name.ToLowerInvariant().Replace(" ", "-").Replace("&", "and");
        var slug = baseSlug;
        var counter = 1;

        while (await _context.Categories.AnyAsync(c => c.Slug == slug, ct))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}
