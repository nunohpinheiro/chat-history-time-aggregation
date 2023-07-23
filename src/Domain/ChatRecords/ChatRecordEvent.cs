using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.ChatRecords;

public record ChatRecordEvent
{
    public readonly EventType EventType;
    public readonly NonEmptyString MinuteFormat;
    public readonly NonEmptyString HourFormat;
    public readonly PositiveInt Day;
    public readonly PositiveInt Month;
    public readonly PositiveInt Year;
    public readonly NonEmptyString User;
    public readonly NonEmptyString MinuteEvent;
    public readonly UtcDateTime Timestamp;

    public ChatRecordEvent(
        EventType eventType,
        UtcDateTime timestamp,
        string user,
        string commentText = null!,
        string highFivedPerson = null!)
    {
        EventType = eventType;
        Timestamp = timestamp;
        User = user;
        MinuteEvent = GetMinuteEventPhrase(commentText, highFivedPerson);

        MinuteFormat = GetTimeAmPm("{0:hh:mm tt}", Timestamp);
        HourFormat = GetTimeAmPm("{0:hh tt}", Timestamp);
        Day = Timestamp.Value.Day;
        Month = Timestamp.Value.Month;
        Year = Timestamp.Value.Year;
    }

    private static string GetTimeAmPm(string format, DateTime timestamp)
        => $"{string.Format(format, timestamp)}".Trim()
        + (timestamp.Hour < 12 ? "am" : "pm");

    private string GetMinuteEventPhrase(string commentText, string highFivedPerson)
        => EventType switch
        {
            EventType.EnterRoom => "enters the room",
            EventType.LeaveRoom => "leaves",
            EventType.Comment when string.IsNullOrWhiteSpace(commentText)
                => throw new ArgumentException($"Comment text must be defined to create a {EventType} event", nameof(commentText)),
            EventType.Comment => $"comments: \"{commentText}\"",
            EventType.HighFiveOtherUser when string.IsNullOrWhiteSpace(highFivedPerson)
                => throw new ArgumentException($"High-fived person must be defined to create a {EventType} event", nameof(highFivedPerson)),
            EventType.HighFiveOtherUser => $"high-fives \"{highFivedPerson}\"",
            _ => throw new NotImplementedException($"Event type {EventType} is not mapped to a minute event phrase")
        };
}
