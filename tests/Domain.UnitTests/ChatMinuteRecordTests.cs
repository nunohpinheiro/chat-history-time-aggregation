namespace ChatHistory.Domain.UnitTests;

[UsesVerify]
public class ChatMinuteRecordTests : SnapshotTestsBase
{
    [Fact]
    public async Task ToText_SnapshotIsMatched()
    {
        ChatMinuteRecord arrangedRecord = new()
        {
            MinuteEvent = "did a sample minute event",
            MinuteFormat = "14:37 PM",
            Timestamp = new DateTime(1993, 2, 4, 14, 37, 0),
            User = "John Doe"
        };

        arrangedRecord.Should().Match<ChatMinuteRecord>(
            r => r.Day == r.Timestamp.Value.Day
            && r.Month == r.Timestamp.Value.Month
            && r.Year == r.Timestamp.Value.Year);

        await Verify(arrangedRecord.ToText())
            .UseFileName($"{nameof(ChatMinuteRecordTests)}_{nameof(ToText_SnapshotIsMatched)}")
            .UseDirectory(SnapshotFilesPath);
    }
}
