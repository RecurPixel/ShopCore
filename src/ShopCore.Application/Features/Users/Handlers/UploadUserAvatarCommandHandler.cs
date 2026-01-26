namespace ShopCore.Application.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand, string>
{
    public Task<string> Handle(UploadUserAvatarCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(string.Empty);
    }
}
