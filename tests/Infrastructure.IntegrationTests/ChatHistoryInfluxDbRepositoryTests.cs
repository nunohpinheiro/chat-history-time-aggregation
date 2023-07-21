using ChatHistory.Domain;

namespace ChatHistory.Infrastructure.IntegrationTests;

public class ChatHistoryInfluxDbRepositoryTests : IClassFixture<ChatHistoryInfluxDbRepositoryTestsFixture>
{
    private static readonly Fixture Fixture = new();
    private readonly ChatHistoryInfluxDbRepository Repository;

    public ChatHistoryInfluxDbRepositoryTests()
    {
        Repository = new(
            ChatHistoryInfluxDbRepositoryTestsFixture.TestOrganizationName,
            ChatHistoryInfluxDbRepositoryTestsFixture.TestBucketName);
    }

    [Fact]
    public async Task AddChatHistoryEvent_ReadChatMinuteRecords_EventsSaved_ReturnInAscendingTime()
    {
        // Arrange
        var today = DateTime.Today;
        var timestamp1 = new DateTime(today.Year, today.Month, today.Day, 9, 30, 0);
        var timestamp2 = timestamp1.AddMinutes(15);

        ChatHistoryEvent chatHistoryEvent1 = new(
            EventType.Comment,
            timestamp1,
            Fixture.Create<string>(),
            commentText: Fixture.Create<string>());

        ChatHistoryEvent chatHistoryEvent2 = new(
            EventType.HighFiveOtherUser,
            timestamp2,
            Fixture.Create<string>(),
            highFivedPerson: Fixture.Create<string>());

        // Act
        await Repository!.AddChatHistoryEvent(chatHistoryEvent2);
        await Repository.AddChatHistoryEvent(chatHistoryEvent1);
        var chatMinuteRecords = await Repository.ReadChatMinuteRecords(1, 50, timestamp1.AddMinutes(-15), timestamp2.AddMinutes(15));

        // Assert
        chatMinuteRecords
            .Should().NotBeNull();
        AssertChatRecordsAreOrdered(chatMinuteRecords);
        AssertChatRecordsMatch(chatMinuteRecords, chatHistoryEvent1, chatHistoryEvent2);
    }

    private static void AssertChatRecordsAreOrdered(IEnumerable<ChatMinuteRecord> chatMinuteRecords)
    {
        for (var i = 0; i < chatMinuteRecords.Count() - 1; i++)
        {
            chatMinuteRecords.ElementAt(i).Timestamp.Value
                .Should().BeBefore(chatMinuteRecords.ElementAt(i + 1).Timestamp.Value);
        }
    }

    private static void AssertChatRecordsMatch(
        IEnumerable<ChatMinuteRecord> chatMinuteRecords, ChatHistoryEvent chatHistoryEvent1, ChatHistoryEvent chatHistoryEvent2)
    {
        AssertRecordsContainEvent(chatMinuteRecords, chatHistoryEvent1);
        AssertRecordsContainEvent(chatMinuteRecords, chatHistoryEvent2);
    }

    private static void AssertRecordsContainEvent(IEnumerable<ChatMinuteRecord> chatMinuteRecords, ChatHistoryEvent chatHistoryEvent)
        => chatMinuteRecords.SingleOrDefault(
            r => r.MinuteEvent == chatHistoryEvent.MinuteEvent
            && r.MinuteFormat == chatHistoryEvent.MinuteFormat
            && r.User == chatHistoryEvent.User
            && r.Day == chatHistoryEvent.Day
            && r.Month == chatHistoryEvent.Month
            && r.Year == chatHistoryEvent.Year)
        .Should()
        .NotBeNull();
}
