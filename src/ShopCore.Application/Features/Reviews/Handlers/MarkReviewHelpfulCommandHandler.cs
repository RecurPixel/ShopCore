namespace ShopCore.Application.Reviews.Commands.MarkReviewHelpful;

public class MarkReviewHelpfulCommandHandler : IRequestHandler<MarkReviewHelpfulCommand>
{
    private readonly IApplicationDbContext _context;

    public MarkReviewHelpfulCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarkReviewHelpfulCommand request, CancellationToken ct)
    {
        var review = await _context.Reviews.FindAsync(request.Id);
        if (review == null)
            throw new NotFoundException("Review", request.Id);

        review.HelpfulCount++;
        await _context.SaveChangesAsync(ct);
    }
}
