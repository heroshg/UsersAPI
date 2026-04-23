using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Tests.Domain;

public class PasswordTests
{
    // --- FromPlainText ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpace_FromPlainText_ThrowsDomainException(string? value)
    {
        Assert.Throws<DomainException>(() => Password.FromPlainText(value!));
    }

    [Fact]
    public void TooShort_FromPlainText_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Password.FromPlainText("Ab1!xy"));
        Assert.Contains("8", ex.Message);
    }

    [Fact]
    public void NoLetter_FromPlainText_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Password.FromPlainText("12345678!"));
        Assert.Contains("letter", ex.Message);
    }

    [Fact]
    public void NoDigit_FromPlainText_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Password.FromPlainText("Abcdefg!"));
        Assert.Contains("digit", ex.Message);
    }

    [Fact]
    public void NoSpecialChar_FromPlainText_ThrowsDomainException()
    {
        var ex = Assert.Throws<DomainException>(() => Password.FromPlainText("Abcdefg1"));
        Assert.Contains("special", ex.Message);
    }

    [Fact]
    public void Valid_FromPlainText_ReturnsPasswordWithValue()
    {
        var pwd = Password.FromPlainText("Secure@1");
        Assert.Equal("Secure@1", pwd.Value);
    }

    // --- FromHash ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpace_FromHash_ThrowsDomainException(string? hash)
    {
        Assert.Throws<DomainException>(() => Password.FromHash(hash!));
    }

    [Fact]
    public void ValidHash_FromHash_ReturnsPasswordWithHash()
    {
        var pwd = Password.FromHash("argon2id.hashed.value");
        Assert.Equal("argon2id.hashed.value", pwd.Value);
    }
}
