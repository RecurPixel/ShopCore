namespace ShopCore.Application.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand, string>
{
    public Task<string> Handle(UploadUserAvatarCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Validate image file (size, format, etc.)
        // 3. Upload file to cloud storage (S3, Azure Blob, etc.)
        // 4. Delete old avatar if exists
        // 5. Update user avatar URL in database
        // 6. Save changes
        // 7. Return new avatar URL
        return Task.FromResult(string.Empty);
    }
}
