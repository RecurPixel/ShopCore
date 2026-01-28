namespace ShopCore.Application.Payouts.Commands.ProcessPayout;

public record ProcessPayoutCommand(int PayoutId) : IRequest;
