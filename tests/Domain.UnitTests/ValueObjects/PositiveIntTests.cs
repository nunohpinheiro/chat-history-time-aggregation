using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.UnitTests.ValueObjects;

public class PositiveIntTests
{
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    public void From_InvalidInt_ThrowsArgumentException(int input)
    {
        var act = () => PositiveInt.From(input);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void From_ValidInt_CreatesPositiveInt(int input)
        => PositiveInt.From(input)
        .Should().Match<PositiveInt>(s => s.Value == input);

    [Theory]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    public void TryFrom_ReturnsExpectedBooleanResult(int value, bool expectedResult)
        => PositiveInt.TryFrom(value, out _)
        .Should().Be(expectedResult);
}
