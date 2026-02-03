using ShopCore.Application.Reviews.DTOs;

namespace ShopCore.Application.Reviews.Commands.RespondToReview;

public class RespondToReviewCommandHandler : IRequestHandler<RespondToReviewCommand, ReviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public RespondToReviewCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<ReviewDto> Handle(RespondToReviewCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, ct);

        if (review == null)
            throw new NotFoundException("Review", request.ReviewId);

        // Verify vendor owns the product
        if (review.Product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only respond to reviews of your products");

        review.VendorResponse = request.VendorResponse;
        review.VendorRespondedAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

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
