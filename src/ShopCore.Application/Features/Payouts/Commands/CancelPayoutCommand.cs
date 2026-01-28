namespace ShopCore.Application.Payouts.Commands.CancelPayout;

public record CancelPayoutCommand(int PayoutId) : IRequest;
