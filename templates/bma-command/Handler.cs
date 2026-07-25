namespace BusinessTemplate.Modules.SampleModule.Features.SampleCommand;
public sealed class Handler { public Task<Response> HandleAsync(Command command, CancellationToken cancellationToken) => Task.FromResult(new Response("Implemented")); }
