using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.ChatRecords;

public interface IChatHistoryRepository
{
    Task AddChatHistoryEvent(ChatRecordEvent chatHistoryEvent, CancellationToken cancellationToken = default);
    Task<List<ChatAggregateRecord>> ReadChatAggregateRecords(
        Granularity granularity, PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange, CancellationToken cancellationToken = default);
    Task<List<ChatMinuteRecord>> ReadChatMinuteRecords(
        PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange, CancellationToken cancellationToken = default);
}
