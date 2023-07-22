using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.ChatRecords;

public record ChatMinuteRecord
{
    public required string MinuteEvent { get; init; }
    public required string MinuteFormat { get; init; }
    public required UtcDateTime Timestamp { get; init; }
    public required string User { get; init; }

    public int Day => Timestamp.Value.Day;
    public int Month => Timestamp.Value.Month;
    public int Year => Timestamp.Value.Year;

    public string ToText()
        => $"{Year}-{Month}-{Day}, {MinuteFormat}: {User} {MinuteEvent}";
}
