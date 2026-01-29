namespace ShopCore.Application.Vendors.Commands.SuspendVendor;

public class SuspendVendorCommandHandler : IRequestHandler<SuspendVendorCommand>
{
    public Task Handle(SuspendVendorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Find vendor by id
        // 2. Mark vendor as suspended
        // 3. Hide vendor's products from public
        // 4. Pause vendor's subscriptions
        // 5. Notify vendor of suspension
        // 6. Log suspension reason
        // 7. Save changes to database
        return Task.CompletedTask;
    }
}
