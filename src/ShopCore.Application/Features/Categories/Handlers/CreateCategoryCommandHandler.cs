namespace ShopCore.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
{
    public Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
