using BusinessTemplate.Modules.Identity.Domain;
using FluentAssertions;

namespace BusinessTemplate.Modules.Identity.Tests;

public sealed class UserDomainTests
{
    [Fact]
    public void Should_create_user()
    {
        var phone = PhoneNumber.Create("+989121234567").Value!;
        var email = EmailAddress.CreateOptional("a@b.com").Value;
        var result = User.Register(new UserId(Guid.NewGuid()), "John", phone, email);

        result.IsSuccess.Should().BeTrue();
        result.Value!.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Should_fail_invalid_phone()
    {
        var result = PhoneNumber.Create("abc");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Should_not_suspend_deleted_user()
    {
        var user = User.Register(new UserId(Guid.NewGuid()), "John", PhoneNumber.Create("+989121234567").Value!, null).Value!;
        user.Delete();
        user.Suspend().IsFailure.Should().BeTrue();
    }
}
