namespace ChatHistory.Infrastructure.UnitTests;

public class ChatHistoryInfluxDbRepositoryTests
{
    [Fact]
    public async Task GetMinuteChatEvents_HasEvents_ReturnsEventsAscendingTimeOrder()
    {
        ChatHistoryInfluxDbRepository repository = new();

        var chatRecords = await repository.GetMinuteChatEvents();

        for (var i = 0; i < chatRecords.Count()-1; i++)
        {
            chatRecords.ElementAt(i).Timestamp
                .Should().BeBefore(chatRecords.ElementAt(i + 1).Timestamp);
        }
    }
}
