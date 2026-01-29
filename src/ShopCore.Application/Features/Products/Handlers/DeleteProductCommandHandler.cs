namespace ShopCore.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    public Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current vendor from context
        // 2. Find product by id
        // 3. Verify vendor owns the product
        // 4. Check if product has active subscriptions (prevent or transfer)
        // 5. Delete/archive product images
        // 6. Remove product from database
        // 7. Save changes
        return Task.CompletedTask;
    }
}
