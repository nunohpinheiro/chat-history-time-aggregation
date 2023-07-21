using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain;

public interface IChatHistoryRepository
{
    Task AddChatHistoryEvent(ChatHistoryEvent chatHistoryEvent);
    Task<List<ChatAggregateRecord>> ReadChatAggregateRecords(
        Granularity granularity, PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange);
    Task<List<ChatMinuteRecord>> ReadChatMinuteRecords(
        PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange);
}
