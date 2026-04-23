using Users.Domain.Exceptions;
using Users.Domain.ValueObjects;

namespace Users.Tests.Domain;

public class EmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NullOrWhiteSpace_Create_ThrowsDomainException(string? address)
    {
        Assert.Throws<DomainException>(() => new Email(address!));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void InvalidFormat_Create_ThrowsDomainException(string address)
    {
        Assert.Throws<DomainException>(() => new Email(address));
    }

    [Fact]
    public void ValidAddress_Create_SetsNormalizedAddress()
    {
        var email = new Email("User@Example.COM");
        Assert.Equal("user@example.com", email.Address);
    }

    [Fact]
    public void SameAddress_Equality_ReturnsTrue()
    {
        var a = new Email("user@example.com");
        var b = new Email("USER@EXAMPLE.COM");
        Assert.Equal(a, b);
        Assert.True(a == b);
    }

    [Fact]
    public void DifferentAddress_Equality_ReturnsFalse()
    {
        var a = new Email("user@example.com");
        var b = new Email("other@example.com");
        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }
}
