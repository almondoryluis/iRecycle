using FastEndpoints;

namespace Uagrm.Sw1.iRecycle.Bff.Api.Endpoints.Home;

public class Home : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }
    public override Task HandleAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
