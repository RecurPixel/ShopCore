using ShopCore.Application.Subscriptions.Commands.AddOneTimeItemToSubscription;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Handlers;

public class AddOneTimeItemToSubscriptionCommandHandler
    : IRequestHandler<AddOneTimeItemToSubscriptionCommand, SubscriptionItemResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddOneTimeItemToSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SubscriptionItemResultDto> Handle(
        AddOneTimeItemToSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var subscription = await _context.Subscriptions
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Subscription), request.SubscriptionId);

        if (subscription.Status != SubscriptionStatus.Active)
        {
            throw new InvalidOperationException("Can only add items to active subscriptions");
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == subscription.VendorId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        if (request.DeliveryDate.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Delivery date must be in the future");
        }

        var subscriptionItem = new SubscriptionItem
        {
            SubscriptionId = subscription.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            IsRecurring = false,
            OneTimeDeliveryDate = request.DeliveryDate,
            IsDelivered = false
        };

        _context.SubscriptionItems.Add(subscriptionItem);
        await _context.SaveChangesAsync(cancellationToken);

        return new SubscriptionItemResultDto(
            subscriptionItem.Id,
            subscriptionItem.ProductId,
            product.Name,
            subscriptionItem.Quantity,
            subscriptionItem.UnitPrice,
            subscriptionItem.IsRecurring,
            subscriptionItem.OneTimeDeliveryDate,
            subscriptionItem.IsDelivered
        );
    }
}
