namespace ShopCore.Application.Vendors.Commands.ApproveVendor;

public class ApproveVendorCommandHandler : IRequestHandler<ApproveVendorCommand>
{
    public Task Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Find vendor by id
        // 2. Verify vendor status is pending
        // 3. Approve vendor account
        // 4. Mark documents as verified
        // 5. Send approval notification to vendor
        // 6. Save changes to database
        return Task.CompletedTask;
    }
}
