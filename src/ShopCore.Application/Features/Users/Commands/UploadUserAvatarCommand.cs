using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Application.Users.Commands.UploadUserAvatar;

public record UploadUserAvatarCommand(IFile AvatarFile) : IRequest<string>;
