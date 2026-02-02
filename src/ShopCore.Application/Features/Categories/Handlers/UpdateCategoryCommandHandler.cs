using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can update categories");

        var category = await _context.Categories.FindAsync(request.Id);
        if (category == null || category.IsDeleted)
            throw new NotFoundException("Category", request.Id);

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.ImageUrl = request.ImageUrl;
        category.DisplayOrder = request.DisplayOrder;

        await _context.SaveChangesAsync(ct);

        return new CategoryDto(category.Id, category.Name, category.Slug, category.Description, category.ImageUrl, null, category.ParentCategoryId, null, category.DisplayOrder, category.IsDeleted, 0);
    }
}
