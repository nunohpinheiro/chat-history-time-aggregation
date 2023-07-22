namespace ChatHistory.Domain.ChatRecords;

public record ChatAggregateRecord
{
    public required int Count { get; init; }
    public required EventType EventType { get; init; }
    public required Granularity Granularity { get; init; }
    public required string? HourFormat { get; init; }
    public required int? Day { get; init; }
    public required int? Month { get; init; }
    public required int Year { get; init; }

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
            _ => $"{Count} {PersonPhrase} {EventPhrase}"
        };

    private string EventPhrase
        => EventType switch
        {
            EventType.EnterRoom => "entered",
            EventType.LeaveRoom => "left",
            EventType.Comment => Count > 1 ? "comments" : "comment",
            EventType.HighFiveOtherUser => "high-fived another person",
            _ => throw new NotImplementedException($"Event type {EventType} is not mapped to an event phrase")
        };

    private string PersonPhrase
        => Count switch
        {
            1 => "person",
            > 1 => "people",
            _ => throw new NotImplementedException($"Count {Count} is not mapped to an event phrase")
        };
}
