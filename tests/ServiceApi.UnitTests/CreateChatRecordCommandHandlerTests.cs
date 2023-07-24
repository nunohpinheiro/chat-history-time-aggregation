using ChatHistory.Domain.ChatRecords;
using ChatHistory.ServiceApi.ChatRecords;
using OneOf.Types;

namespace ChatHistory.ServiceApi.UnitTests;

public class CreateChatRecordCommandHandlerTests
{
    private readonly Fixture Fixture = new();
    private readonly Mock<IChatHistoryRepository> RepositoryMock = new(MockBehavior.Strict);
    private readonly CreateChatRecordCommandHandler Handler;

    public CreateChatRecordCommandHandlerTests()
    {
        Handler = new(
            new LoggerFixtureMock<CreateChatRecordCommandHandler>(),
            new CreateChatRecordCommandValidator(),
            RepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_InvalidInputs_ReturnsValidationFailures()
    {
        // Arrange
        var invalidCommand = Fixture.Create<CreateChatRecordCommand>();

        // Act
        var result = await Handler.Handle(invalidCommand);

        // Assert
        result.AsT1.Should().NotBeNullOrEmpty();

        RepositoryMock.Verify(
            r => r.AddChatHistoryEvent(It.IsAny<ChatRecordEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidInputs_CallsChatHistoryRepository()
    {
        // Arrange
        CreateChatRecordCommand validCommand = new(
            "comment",
            "2023-07-17T00:00:00Z",
            Fixture.Create<string>(),
            Fixture.Create<string>(),
            Fixture.Create<string>());

        RepositoryMock
            .Setup(r => r.AddChatHistoryEvent(It.IsAny<ChatRecordEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await Handler.Handle(validCommand);

        // Assert
        result.AsT0.Should().BeEquivalentTo(new Success());

        RepositoryMock.Verify(
            r => r.AddChatHistoryEvent(It.IsAny<ChatRecordEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
