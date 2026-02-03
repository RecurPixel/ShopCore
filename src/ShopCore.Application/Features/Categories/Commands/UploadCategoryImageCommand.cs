using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Application.Categories.Commands.UploadCategoryImage;

public record UploadCategoryImageCommand(int CategoryId, IFile AvatarFile) : IRequest<string>;
