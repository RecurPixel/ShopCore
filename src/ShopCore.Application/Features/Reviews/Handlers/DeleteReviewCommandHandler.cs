namespace ShopCore.Application.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteReviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews.FindAsync(request.Id);

        if (review == null)
            throw new NotFoundException("Review", request.Id);

        // Can delete own review or admin can delete any
        if (review.UserId != _currentUser.UserId && _currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("You can only delete your own reviews");

        review.IsDeleted = true;
        await _context.SaveChangesAsync(ct);

        // Recalculate product rating
        var product = await _context.Products.FindAsync(review.ProductId);
        if (product != null)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == review.ProductId && !r.IsDeleted)
                .ToListAsync(ct);

            product.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            product.ReviewCount = reviews.Count;
        }

        await _context.SaveChangesAsync(ct);
    }
}
