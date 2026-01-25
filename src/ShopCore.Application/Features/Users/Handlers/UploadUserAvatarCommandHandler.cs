namespace ShopCore.Application.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand>
{
    public Task Handle(UploadUserAvatarCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
