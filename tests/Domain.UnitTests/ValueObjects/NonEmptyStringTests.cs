using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.UnitTests.ValueObjects;

public class NonEmptyStringTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void From_InvalidString_ThrowsArgumentException(string value)
    {
        var act = () => NonEmptyString.From(value);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_ValidString_ReturnsNonEmptyString()
    {
        var originString = new Fixture().Create<string>();

        NonEmptyString.From(originString)
            .Should().Match<NonEmptyString>(
            s => s.Value == originString);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("sample-string", true)]
    public void TryFrom_ReturnsExpectedBooleanResult(string value, bool expectedResult)
        => NonEmptyString.TryFrom(value, out _)
        .Should().Be(expectedResult);
}
