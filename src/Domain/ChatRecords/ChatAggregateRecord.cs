namespace ChatHistory.Domain.ChatRecords;

public record ChatAggregateRecord
{
    public required int Count { get; init; }
    public required Granularity Granularity { get; init; }
    public required string? HourFormat { get; init; }
    public required int? Day { get; init; }
    public required int? Month { get; init; }
    public required int Year { get; init; }
    public required string DashedEventType { private get; init; }

    public string Order => $"{Year}{Month}{Day}{HourFormat}";

    public EventType EventType
    {
        get
        {
            if (DashedEventType.TryGetEventType(out var eventType))
                return eventType;

            var (highFiveEvent, _, _) = DashedEventType.GetHighFiveEventDetails();
            return highFiveEvent;
        }
    }

    public int? HighFiveReceiversCount { get; private set; }

    public (string highFiveGiver, string highFiveReceiver) HighFiveUsers
    {
        get
        {
            var (_, highFiveGiver, highFiveReceiver) = DashedEventType.GetHighFiveEventDetails();
            return (highFiveGiver, highFiveReceiver);
        }
    }

    public void SetHighFiveReceiversCount(int highFiveReceiversCount)
    {
        HighFiveReceiversCount = highFiveReceiversCount;
    }

    public string ToText()
        => Granularity switch
        {
            Granularity.Hourly => $"{Year}-{Month}-{Day}, {HourFormat}: {AggregatedEventPhrase}",
            Granularity.Daily => $"{Year}-{Month}-{Day}: {AggregatedEventPhrase}",
            Granularity.Monthly => $"{Year}-{Month}: {AggregatedEventPhrase}",
            Granularity.Yearly => $"{Year}: {AggregatedEventPhrase}",
            _ => throw new NotImplementedException($"Granularity {Granularity} is not mapped to a text phrase")
        };

    private string AggregatedEventPhrase
        => EventType switch
        {
            EventType.Comment => $"{Count} {EventPhrase}",
            _ => $"{Count} {GetPersonPhrase(Count)} {EventPhrase}"
        };

    private string EventPhrase
        => EventType switch
        {
            EventType.EnterRoom => "entered",
            EventType.LeaveRoom => "left",
            EventType.Comment => Count > 1 ? "comments" : "comment",
            EventType.HighFiveOtherUser => $"high-fived {HighFiveReceiversCount} other {GetPersonPhrase((int)HighFiveReceiversCount!)}",
            _ => throw new NotImplementedException($"Event type {EventType} is not mapped to an event phrase")
        };

    private static string GetPersonPhrase(int count)
        => count switch
        {
            1 => "person",
            > 1 => "people",
            _ => throw new NotImplementedException($"Count {count} is not mapped to an event phrase")
        };
}
