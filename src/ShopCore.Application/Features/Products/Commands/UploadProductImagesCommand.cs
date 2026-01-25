using ShopCore.Application.Common.Interfaces;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UploadProductImages;

public record UploadProductImagesCommand(int ProductId, List<IFile> Images)
    : IRequest<List<ProductImageDto>>;
