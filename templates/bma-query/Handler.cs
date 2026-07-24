namespace BusinessTemplate.Modules.SampleModule.Features.SampleQuery;
public sealed class Handler { public Task<Response?> HandleAsync(Query query, CancellationToken cancellationToken) => Task.FromResult<Response?>(new Response(query.Id, "Sample")); }
