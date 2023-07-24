using Ardalis.GuardClauses;
using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using ChatHistory.ServiceApi.Pipelines;
using FluentValidation;

namespace ChatHistory.ServiceApi.ChatRecords;

internal class CreateChatRecordCommandHandler : CommandHandlerPipeline<CreateChatRecordCommand>
{
    private readonly IChatHistoryRepository ChatHistoryRepository;

    public CreateChatRecordCommandHandler(
        ILogger<CreateChatRecordCommandHandler> logger,
        IValidator<CreateChatRecordCommand> commandValidator,
        IChatHistoryRepository chatHistoryRepository)
        : base(logger, commandValidator)
    {
        ChatHistoryRepository = Guard.Against.Null(chatHistoryRepository);
    }

    protected override async Task Execute(CreateChatRecordCommand command, CancellationToken cancellationToken = default)
    {
        command.Timestamp.TryGetUtcDateTime(out UtcDateTime timestamp);

        await ChatHistoryRepository.AddChatHistoryEvent(
            new ChatRecordEvent(
                command.EventType.ToEventType(),
                timestamp,
                command.User,
                command.CommentText,
                command.HighFivedPerson),
            cancellationToken);
    }
}
