using FastEndpoints;

namespace BusinessTemplate.Modules.SampleModule.Features.SampleQuery;

public sealed class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/sample/module/sample-query/{id:guid}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await Send.OkAsync(new Response(req.Id, "Sample"), ct);
    }
}
