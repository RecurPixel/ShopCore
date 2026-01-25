namespace ShopCore.Application.Users.Commands.UpdateCurrentUser;

public class UpdateCurrentUserCommandHandler : IRequestHandler<UpdateCurrentUserCommand>
{
    public Task Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
