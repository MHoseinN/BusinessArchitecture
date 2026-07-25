using NetArchTest.Rules;

namespace BusinessTemplate.ArchitectureTests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_should_not_depend_on_ef_or_fastendpoints()
    {
        var result = Types.InAssembly(typeof(BusinessTemplate.Modules.Identity.Domain.User).Assembly)
            .That()
            .ResideInNamespace("BusinessTemplate.Modules.Identity.Domain")
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "FastEndpoints")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}
