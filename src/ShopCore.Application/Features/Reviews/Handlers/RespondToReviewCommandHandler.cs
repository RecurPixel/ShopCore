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

    public async Task Handle(RespondToReviewCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (review == null)
            throw new NotFoundException("Review", request.Id);

        // Verify vendor owns the product
        if (review.Product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only respond to reviews of your products");

        review.VendorResponse = request.Response;
        review.VendorRespondedAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }
}
