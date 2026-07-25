using FastEndpoints;
using FluentValidation;

namespace BusinessTemplate.Modules.SampleModule.Features.SampleCommand;

public sealed class Validator : Validator<Request>
{
    public Validator() => RuleFor(x => x.Name).NotEmpty();
}
