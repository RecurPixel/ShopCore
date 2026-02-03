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

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (review == null || review.UserId != _currentUser.UserId)
            throw new NotFoundException("Review", request.Id);

        review.Rating = request.Rating;
        review.Title = request.Title;
        review.Comment = request.Comment;

        await _context.SaveChangesAsync(ct);

        // Recalculate product rating
        var product = review.Product;
        if (product != null)
        {
            var avgRating = await _context.Reviews
                .Where(r => r.ProductId == review.ProductId)
                .AverageAsync(r => r.Rating, ct);

            product.AverageRating = avgRating;
            await _context.SaveChangesAsync(ct);
        }

        return new ReviewDto(
            Id: review.Id,
            ProductId: review.ProductId,
            ProductName: review.Product.Name,
            UserId: review.UserId,
            UserName: review.User.FullName,
            UserAvatarUrl: review.User.AvatarUrl,
            Rating: review.Rating,
            Title: review.Title,
            Comment: review.Comment,
            ImageUrls: review.Images.Select(i => i.ImageUrl).ToList(),
            IsVerifiedPurchase: review.IsVerifiedPurchase,
            HelpfulCount: review.HelpfulCount,
            VendorResponse: review.VendorResponse,
            VendorRespondedAt: review.VendorRespondedAt,
            CreatedAt: review.CreatedAt
        );
    }
}
