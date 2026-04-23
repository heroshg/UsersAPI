using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Tests.Domain;

public class RoleTests
{
    [Theory]
    [InlineData("User")]
    [InlineData("user")]
    [InlineData("USER")]
    public void UserVariants_From_ReturnsUserRole(string value)
    {
        var role = Role.From(value);
        Assert.Equal(Role.User.Value, role.Value);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("admin")]
    [InlineData("ADMIN")]
    public void AdminVariants_From_ReturnsAdminRole(string value)
    {
        var role = Role.From(value);
        Assert.Equal(Role.Admin.Value, role.Value);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("moderator")]
    [InlineData("")]
    [InlineData("   ")]
    public void InvalidRole_From_ThrowsDomainException(string value)
    {
        Assert.Throws<DomainException>(() => Role.From(value));
    }
}
