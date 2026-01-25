namespace ShopCore.Application.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(int Id) : IRequest<Unit>;
