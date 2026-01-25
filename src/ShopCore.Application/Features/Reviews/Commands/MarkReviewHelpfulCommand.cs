namespace ShopCore.Application.Reviews.Commands.MarkReviewHelpful;

public record MarkReviewHelpfulCommand(int ReviewId) : IRequest<Unit>;
