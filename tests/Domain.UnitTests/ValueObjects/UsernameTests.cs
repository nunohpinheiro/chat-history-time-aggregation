using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.UnitTests.ValueObjects;

public class UsernameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("#")]
    [InlineData("&/()")]
    public void From_InvalidString_ThrowsArgumentException(string value)
    {
        var act = () => Username.From(value);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_ValidString_ReturnsNonEmptyString()
    {
        var originString = "example @_." + new Fixture().Create<Guid>().ToString();

        Username.From(originString)
            .Should().Match<Username>(
            s => s.Value == originString);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("sample#string", false)]
    [InlineData("sample string-sample@sample_sample.125", true)]
    public void TryFrom_ReturnsExpectedBooleanResult(string value, bool expectedResult)
        => Username.TryFrom(value, out _)
        .Should().Be(expectedResult);
}
