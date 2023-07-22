namespace ChatHistory.Domain.UnitTests;

[UsesVerify]
public class ChatHistoryEventTests : SnapshotTestsBase
{
    private readonly Fixture Fixture = new();

    [Theory]
    [InlineData(EventType.EnterRoom)]
    [InlineData(EventType.LeaveRoom)]
    [InlineData(EventType.Comment)]
    [InlineData(EventType.HighFiveOtherUser)]
    public async Task Ctor_ValidInputs_SnapshotIsMatched(EventType eventType)
    {
        ChatHistoryEvent arrangedRecord = new(
            eventType,
            new DateTime(1993, 2, 4, 14, 37, 0),
            "John Doe",
            "This is a comment only used in comments",
            "Person For High-Fives");

        await Verify(arrangedRecord)
            .DontScrubDateTimes()
            .UseFileName($"{nameof(ChatHistoryEventTests)}_{nameof(Ctor_ValidInputs_SnapshotIsMatched)}_{eventType}")
            .UseDirectory(SnapshotFilesPath);
    }

    [Theory]
    [InlineData(EventType.Comment)]
    [InlineData(EventType.HighFiveOtherUser)]
    public void Ctor_CommentOrHighFive_WithoutOptionalInputs_ThrowsArgumentException(EventType eventType)
    {
        var act = () => new ChatHistoryEvent(eventType, Fixture.Create<DateTime>(), Fixture.Create<string>());
        act.Should().Throw<ArgumentException>();
    }
}
