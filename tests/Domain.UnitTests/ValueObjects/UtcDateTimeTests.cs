using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.UnitTests.ValueObjects;

public class UtcDateTimeTests
{
    [Fact]
    public void From_UnspecifiedKindDateTime_ReturnsUtcDateTime()
    {
        var unspecifiedDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        UtcDateTime.From(unspecifiedDateTime)
            .Should().Match<UtcDateTime>(
            u => u.Value.Kind == DateTimeKind.Utc);
    }
}
