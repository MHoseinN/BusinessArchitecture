namespace BusinessTemplate.Modules.SampleModule.Features.SampleQuery;
public static class Mapper { public static Query ToQuery(Request request) => new(request.Id); }
