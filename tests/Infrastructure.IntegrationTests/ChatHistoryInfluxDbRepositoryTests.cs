using ChatHistory.Domain.ChatRecords;
using ChatHistory.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace ChatHistory.Infrastructure.IntegrationTests;

public class ChatHistoryInfluxDbRepositoryTests : IClassFixture<ChatHistoryInfluxDbRepositoryTestsFixture>
{
    private static readonly Fixture Fixture = new();
    private readonly ChatHistoryInfluxDbRepository Repository;

    public ChatHistoryInfluxDbRepositoryTests()
    {
        InfluxDbSettingsOptions influxDbOptions = new()
        {
            Bucket = ChatHistoryInfluxDbRepositoryTestsFixture.TestBucketName,
            Measurement = "chat-history",
            Organization = ChatHistoryInfluxDbRepositoryTestsFixture.TestOrganizationName,
            Token = "myadmintoken",
            Url = "http://localhost:8086"
        };

        Repository = new(Options.Create(influxDbOptions));
    }

    [Fact]
    public async Task AddChatHistoryEvent_ReadChatMinuteRecords_EventsSaved_ReturnInAscendingTime()
    {
        // Arrange
        var today = DateTime.Today;
        var timestamp1 = new DateTime(today.Year, today.Month, today.Day, 9, 30, 0);
        var timestamp2 = timestamp1.AddMinutes(15);

        var chatHistoryEvent1 = ArrangeChatHistoryEvent(Fixture.Create<EventType>(), timestamp1);
        var chatHistoryEvent2 = ArrangeChatHistoryEvent(Fixture.Create<EventType>(), timestamp2);

        // Act
        await Repository!.AddChatHistoryEvent(chatHistoryEvent2);
        await Repository.AddChatHistoryEvent(chatHistoryEvent1);
        var chatMinuteRecords = await Repository.ReadChatMinuteRecords(1, 50, timestamp1.AddMinutes(-15), timestamp2.AddMinutes(15));

        // Assert
        chatMinuteRecords
            .Should().NotBeNull();
        AssertChatRecordsAreOrdered(chatMinuteRecords);
        AssertRecordsContainEvent(chatMinuteRecords, chatHistoryEvent1);
        AssertRecordsContainEvent(chatMinuteRecords, chatHistoryEvent2);
    }

    [Theory]
    [InlineData(Granularity.Hourly, EventType.EnterRoom, 12)]
    [InlineData(Granularity.Daily, EventType.LeaveRoom, 14)]
    [InlineData(Granularity.Monthly, EventType.Comment, 16)]
    [InlineData(Granularity.Yearly, EventType.HighFiveOtherUser, 18)]
    public async Task AddChatHistoryEvent_ReadChatAggregateRecords_EventsSaved_ReturnInAscendingTime(
        Granularity granularity, EventType eventType, int initialHour)
    {
        // Arrange
        var today = DateTime.Today;
        var timestamp1 = new DateTime(today.Year, today.Month, today.Day, initialHour, 10, 0);
        var timestamp2 = timestamp1.AddMinutes(15);

        var chatHistoryEvent1 = ArrangeChatHistoryEvent(eventType, timestamp1);
        var chatHistoryEvent2 = ArrangeChatHistoryEvent(eventType, timestamp2);

        // Act
        await Repository!.AddChatHistoryEvent(chatHistoryEvent2);
        await Repository.AddChatHistoryEvent(chatHistoryEvent1);
        var chatAggregateRecords = await Repository.ReadChatAggregateRecords(granularity, 1, 50, timestamp1.AddMinutes(-15), timestamp2.AddMinutes(15));

        // Assert
        chatAggregateRecords
            .Should().NotBeNull();
        AssertChatRecordsAreOrdered(chatAggregateRecords);
        AssertRecordsContainEvent(chatAggregateRecords, chatHistoryEvent1, granularity);
        AssertRecordsContainEvent(chatAggregateRecords, chatHistoryEvent2, granularity);
    }

    private static ChatRecordEvent ArrangeChatHistoryEvent(EventType eventType, DateTime timestamp)
        => eventType switch
        {
            EventType.Comment => new ChatRecordEvent(
                eventType, timestamp, Fixture.Create<string>(), commentText: Fixture.Create<string>()),
            EventType.HighFiveOtherUser => new ChatRecordEvent(
                eventType, timestamp, Fixture.Create<string>(), highFivedPerson: Fixture.Create<string>()),
            _ => new ChatRecordEvent(eventType, timestamp, Fixture.Create<string>())
        };

    private static void AssertChatRecordsAreOrdered(List<ChatMinuteRecord> chatMinuteRecords)
    {
        for (var i = 0; i < chatMinuteRecords.Count - 1; i++)
        {
            chatMinuteRecords.ElementAt(i).Timestamp.Value
                .Should().BeBefore(chatMinuteRecords.ElementAt(i + 1).Timestamp.Value);
        }
    }

    private static void AssertChatRecordsAreOrdered(List<ChatAggregateRecord> chatAggregateRecords)
    {
        for (var i = 0; i < chatAggregateRecords.Count - 1; i++)
        {
            var record1 = chatAggregateRecords.ElementAt(i);
            var time1 = GetComparableTime(record1);

            var record2 = chatAggregateRecords.ElementAt(i + 1);
            var time2 = GetComparableTime(record2);

            time1.Should().BeLessThanOrEqualTo(time2);
        }

        static int GetComparableTime(ChatAggregateRecord record)
            => int.Parse($"{record.Year}{record.Month}{record.Day}{record.HourFormat}".Split(" ").First());
    }

    private static void AssertRecordsContainEvent(List<ChatMinuteRecord> chatMinuteRecords, ChatRecordEvent chatHistoryEvent)
        => chatMinuteRecords.SingleOrDefault(
            r => r.MinuteEvent == chatHistoryEvent.MinuteEvent
            && r.MinuteFormat == chatHistoryEvent.MinuteFormat
            && r.User == chatHistoryEvent.User
            && r.Day == chatHistoryEvent.Day
            && r.Month == chatHistoryEvent.Month
            && r.Year == chatHistoryEvent.Year)
        .Should()
        .NotBeNull();

    private static void AssertRecordsContainEvent(
        List<ChatAggregateRecord> chatAggregateRecords, ChatRecordEvent chatHistoryEvent, Granularity granularity)
        => chatAggregateRecords.SingleOrDefault(
            r => r.Count >= 1
            && r.EventType == chatHistoryEvent.EventType
            && r.Granularity == granularity
            && r.HourFormat == (granularity == Granularity.Hourly ? chatHistoryEvent.HourFormat.Value : null)
            && r.Day == (granularity <= Granularity.Daily ? chatHistoryEvent.Day.Value : null)
            && r.Month == (granularity <= Granularity.Monthly ? chatHistoryEvent.Month.Value : null)
            && r.Year == (granularity <= Granularity.Yearly ? chatHistoryEvent.Year.Value : null))
        .Should()
        .NotBeNull();
}
