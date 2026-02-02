using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateReviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews.FindAsync(request.Id);

        if (review == null || review.UserId != _currentUser.UserId)
            throw new NotFoundException("Review", request.Id);

        review.Rating = request.Rating;
        review.Title = request.Title;
        review.Comment = request.Comment;

        await _context.SaveChangesAsync(ct);

        // Recalculate product rating
        var product = await _context.Products.FindAsync(review.ProductId);
        if (product != null)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == review.ProductId)
                .ToListAsync(ct);

            product.AverageRating = reviews.Average(r => r.Rating);
        }

        await _context.SaveChangesAsync(ct);
    }
}
