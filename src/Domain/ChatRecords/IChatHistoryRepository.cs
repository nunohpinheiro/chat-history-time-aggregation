using ChatHistory.Domain.ValueObjects;

namespace ChatHistory.Domain.ChatRecords;

public interface IChatHistoryRepository
{
    Task AddChatHistoryEvent(ChatRecordEvent chatHistoryEvent);
    Task<List<ChatAggregateRecord>> ReadChatAggregateRecords(
        Granularity granularity, PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange);
    Task<List<ChatMinuteRecord>> ReadChatMinuteRecords(
        PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange);
}
