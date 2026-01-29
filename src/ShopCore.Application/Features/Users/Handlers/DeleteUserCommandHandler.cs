namespace ShopCore.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Find user by id
        // 2. Check user has no active orders/subscriptions (or archive them)
        // 3. Delete user addresses
        // 4. Delete user wishlist
        // 5. Delete/anonymize user reviews
        // 6. Delete user account
        // 7. Save changes to database
        return Task.CompletedTask;
    }
}
