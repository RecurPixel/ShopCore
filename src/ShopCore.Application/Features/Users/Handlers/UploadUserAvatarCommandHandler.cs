namespace ShopCore.Application.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public UploadUserAvatarCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    public async Task<string> Handle(UploadUserAvatarCommand request, CancellationToken ct)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, ct);

        if (user == null)
            throw new NotFoundException("User", _currentUser.UserId);

        // Delete old avatar if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _fileStorage.DeleteFileAsync(user.AvatarUrl);
        }

        // Upload new avatar
        using var stream = request.AvatarFile.OpenReadStream();
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.AvatarFile.FileName)}";
        var avatarUrl = await _fileStorage.SaveFileAsync(stream, fileName, $"users/{user.Id}/avatar");

        user.AvatarUrl = avatarUrl;
        await _context.SaveChangesAsync(ct);

        return _fileStorage.GetFileUrl(avatarUrl);
    }
}
