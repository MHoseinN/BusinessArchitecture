using FastEndpoints;

namespace BusinessTemplate.Modules.SampleModule.Features.SampleCommand;

public sealed class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/sample/module/sample-command");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await Send.OkAsync(new Response("Implemented"), ct);
    }
}
