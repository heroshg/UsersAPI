using Users.Domain.Entities;
using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Tests.Domain;

public class UserTests
{
    private static Email ValidEmail => new("user@example.com");
    private static Password ValidPassword => Password.FromHash("hash");

    // --- Create ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpaceName_Create_ThrowsDomainException(string? name)
    {
        Assert.Throws<DomainException>(() => User.Create(name!, ValidEmail, ValidPassword));
    }

    [Fact]
    public void NameOver150Chars_Create_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => User.Create(new string('A', 151), ValidEmail, ValidPassword));
    }

    [Fact]
    public void NameWith150Chars_Create_DoesNotThrow()
    {
        var ex = Record.Exception(() => User.Create(new string('A', 150), ValidEmail, ValidPassword));
        Assert.Null(ex);
    }

    [Fact]
    public void NullEmail_Create_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => User.Create("Test User", null!, ValidPassword));
    }

    [Fact]
    public void NullPassword_Create_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => User.Create("Test User", ValidEmail, null!));
    }

    [Fact]
    public void Valid_Create_SetsDefaultsCorrectly()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);

        Assert.Equal("Test User", user.Name.Value);
        Assert.Equal("user@example.com", user.Email.Address);
        Assert.Equal(Role.User.Value, user.Role.Value);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void NameWithLeadingTrailingSpaces_Create_NormalizesName()
    {
        var user = User.Create("  Test User  ", ValidEmail, ValidPassword);
        Assert.Equal("Test User", user.Name.Value);
    }

    // --- ChangeName ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpaceName_ChangeName_ThrowsDomainException(string? name)
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        Assert.Throws<DomainException>(() => user.ChangeName(name!));
    }

    [Fact]
    public void NameOver150Chars_ChangeName_ThrowsDomainException()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        Assert.Throws<DomainException>(() => user.ChangeName(new string('A', 151)));
    }

    [Fact]
    public void ValidName_ChangeName_UpdatesNameAndTimestamp()
    {
        var user = User.Create("Old Name", ValidEmail, ValidPassword);
        var before = user.UpdatedAt;

        user.ChangeName("New Name");

        Assert.Equal("New Name", user.Name.Value);
        Assert.True(user.UpdatedAt >= before);
    }

    // --- ChangeEmail ---

    [Fact]
    public void NullEmail_ChangeEmail_ThrowsDomainException()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        Assert.Throws<DomainException>(() => user.ChangeEmail(null!));
    }

    [Fact]
    public void ValidEmail_ChangeEmail_UpdatesEmailAndTimestamp()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        var before = user.UpdatedAt;

        user.ChangeEmail(new Email("new@example.com"));

        Assert.Equal("new@example.com", user.Email.Address);
        Assert.True(user.UpdatedAt >= before);
    }

    // --- ChangeRole ---

    [Fact]
    public void InvalidRole_ChangeRole_ThrowsDomainException()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        Assert.Throws<DomainException>(() => user.ChangeRole("SuperAdmin"));
    }

    [Fact]
    public void ValidRole_ChangeRole_UpdatesRoleAndTimestamp()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        var before = user.UpdatedAt;

        user.ChangeRole("Admin");

        Assert.Equal(Role.Admin.Value, user.Role.Value);
        Assert.True(user.UpdatedAt >= before);
    }

    // --- ChangePassword ---

    [Fact]
    public void NullPassword_ChangePassword_ThrowsDomainException()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        Assert.Throws<DomainException>(() => user.ChangePassword(null!));
    }

    [Fact]
    public void ValidPassword_ChangePassword_UpdatesPasswordAndTimestamp()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        var newPassword = Password.FromHash("new-hash");
        var before = user.UpdatedAt;

        user.ChangePassword(newPassword);

        Assert.Equal("new-hash", user.Password.Value);
        Assert.True(user.UpdatedAt >= before);
    }

    // --- Deactivate / Activate ---

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        user.Deactivate();
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveTrue()
    {
        var user = User.Create("Test User", ValidEmail, ValidPassword);
        user.Deactivate();
        user.Activate();
        Assert.True(user.IsActive);
    }
}
