namespace ShopCore.Application.Features.Test.Queries;

public record GetTestQuery : IRequest<string>;

public class GetTestQueryHandler : IRequestHandler<GetTestQuery, string>
{
    public Task<string> Handle(GetTestQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult("MediatR is working! 🚀");
    }
}
