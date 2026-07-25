namespace BusinessTemplate.Modules.SampleModule.Features.SampleCommand;
public static class Mapper { public static Command ToCommand(Request request) => new(request.Name); }
