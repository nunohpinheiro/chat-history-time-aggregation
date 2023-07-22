namespace ChatHistory.Domain.UnitTests;

[UsesVerify]
public class ChatAggregateRecordTests : SnapshotTestsBase
{
    [Theory]
    [InlineData(EventType.EnterRoom, Granularity.Hourly)]
    [InlineData(EventType.LeaveRoom, Granularity.Daily)]
    [InlineData(EventType.Comment, Granularity.Monthly)]
    [InlineData(EventType.HighFiveOtherUser, Granularity.Yearly)]
    public async Task ToText_DependsOnEventTypeAndGranularity_SnapshotIsMatched(EventType eventType, Granularity granularity)
    {
        ChatAggregateRecord arrangedRecord = new()
        {
            Count = 837,
            Day = 4,
            EventType = eventType,
            Granularity = granularity,
            HourFormat = "3 PM",
            Month = 2,
            Year = 1993
        };

        await Verify(arrangedRecord.ToText())
            .UseFileName($"{nameof(ChatAggregateRecordTests)}_{nameof(ToText_DependsOnEventTypeAndGranularity_SnapshotIsMatched)}_{eventType}_{granularity}")
            .UseDirectory(SnapshotFilesPath);
    }
}
