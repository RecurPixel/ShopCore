namespace ShopCore.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can delete categories");

        var category = await _context.Categories.FindAsync(request.Id);
        if (category == null)
            throw new NotFoundException("Category", request.Id);

        // Check for products
        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == request.Id && !p.IsDeleted, ct);

        if (hasProducts)
            throw new ValidationException("Cannot delete category with products");

        category.IsDeleted = true;
        await _context.SaveChangesAsync(ct);
    }
}
