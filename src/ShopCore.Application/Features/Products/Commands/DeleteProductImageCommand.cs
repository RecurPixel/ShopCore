namespace ShopCore.Application.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand(int ProductId, int ImageId) : IRequest;
