using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using ChatHistory.ServiceApi.ChatRecords;

namespace ChatHistory.ServiceApi.UnitTests;

public class ReadChatRecordsQueryHandlerTests
{
    private readonly Fixture Fixture = new();
    private readonly Mock<IChatHistoryRepository> RepositoryMock = new(MockBehavior.Strict);
    private readonly ReadChatRecordsQueryHandler Handler;

    public ReadChatRecordsQueryHandlerTests()
    {
        Handler = new(new ReadChatRecordsQueryValidator(), RepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidInputs_ReturnsValidationFailures()
    {
        // Arrange
        var invalidCommand = Fixture.Create<ReadChatRecordsQuery>();

        // Act
        var result = await Handler.Handle(invalidCommand);

        // Assert
        result.AsT1.Should().NotBeNullOrEmpty();

        RepositoryMock.Verify(
            r => r.AddChatHistoryEvent(It.IsAny<ChatRecordEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(Granularity.MinuteByMinute)]
    [InlineData(Granularity.Hourly)]
    [InlineData(Granularity.Daily)]
    [InlineData(Granularity.Monthly)]
    [InlineData(Granularity.Yearly)]
    public async Task Handle_ValidInputs_CallsChatHistoryRepository(Granularity granularity)
    {
        // Arrange
        ReadChatRecordsQuery validQuery = new(
            granularity,
            Fixture.Create<int>(),
            Fixture.Create<int>(),
            "2023-07-17T00:00:00Z",
            "2023-07-17T00:00:00Z");

        if (granularity is Granularity.MinuteByMinute)
            RepositoryMock
                .Setup(r => r.ReadChatMinuteRecords(
                    It.IsAny<PositiveInt>(), It.IsAny<PositiveInt>(), It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ChatMinuteRecord>());
        else
            RepositoryMock
                .Setup(r => r.ReadChatAggregateRecords(
                    granularity, It.IsAny<PositiveInt>(), It.IsAny<PositiveInt>(), It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ChatAggregateRecord>());

        // Act
        var result = await Handler.Handle(validQuery);

        // Assert
        result.AsT0.Should().NotBeNull();

        if (granularity is Granularity.MinuteByMinute)
            RepositoryMock.Verify(
                r => r.ReadChatMinuteRecords(
                    It.IsAny<PositiveInt>(), It.IsAny<PositiveInt>(), It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()),
                Times.Once);
        else
            RepositoryMock.Verify(
                r => r.ReadChatAggregateRecords(
                    granularity, It.IsAny<PositiveInt>(), It.IsAny<PositiveInt>(), It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
