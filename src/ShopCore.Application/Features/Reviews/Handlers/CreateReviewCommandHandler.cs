using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        // Verify purchase
        var hasPurchased = await _context.OrderItems
            .AnyAsync(oi => oi.ProductId == request.ProductId &&
                          oi.Order.UserId == _currentUser.UserId &&
                          oi.Status == OrderStatus.Delivered, ct);

        if (!hasPurchased)
            throw new ValidationException("You can only review products you have purchased");

        // Check if already reviewed
        var existingReview = await _context.Reviews
            .AnyAsync(r => r.ProductId == request.ProductId && r.UserId == _currentUser.UserId, ct);

        if (existingReview)
            throw new ValidationException("You have already reviewed this product");

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = _currentUser.RequiredUserId,
            Rating = request.Rating,
            Title = request.Title,
            Comment = request.Comment,
            ImageUrls = request.ImageUrls != null ? string.Join(",", request.ImageUrls) : null,
            IsVerifiedPurchase = true,
            HelpfulCount = 0
        };

        _context.Reviews.Add(review);

        // Update product rating
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product != null)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == request.ProductId)
                .ToListAsync(ct);

            product.AverageRating = reviews.Average(r => r.Rating);
            product.ReviewCount = reviews.Count + 1;
        }

        await _context.SaveChangesAsync(ct);

        return new ReviewDto { Id = review.Id, Rating = review.Rating, Comment = review.Comment };
    }
}
