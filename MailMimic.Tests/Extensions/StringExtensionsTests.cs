using MailMimic.Extensions;

namespace MailMimic.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("Hello: World", "Hello", "World")]
    [InlineData("Hello: World:123", "Hello", "World:123")]
    [InlineData("Hello: World\t\t", "Hello", "World")]
    public void SplitKeyValue_ValidInput_SplitsString(string input, string expectedKey, string expectedValue)
    {
        // Act
        var (key, value) = input.SplitKeyValue();

        // Assert
        Assert.Equal(expectedKey, key);
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void SplitKeyValue_MissingColon_ThrowsInvalidOperationException()
    {
        // Arrange
        var input = "Hello World";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => input.SplitKeyValue());
    }

    [Fact]
    public void SplitKeyValue_InputIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => StringExtensions.SplitKeyValue(null!));
    }
}
