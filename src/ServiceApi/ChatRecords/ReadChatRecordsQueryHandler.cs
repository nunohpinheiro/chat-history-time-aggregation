using Ardalis.GuardClauses;
using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using ChatHistory.ServiceApi.Pipelines;
using FluentValidation;

namespace ChatHistory.ServiceApi.ChatRecords;

internal class ReadChatRecordsQueryHandler : QueryHandlerPipeline<ReadChatRecordsQuery, IEnumerable<string>>
{
    private readonly IChatHistoryRepository ChatHistoryRepository;

    public ReadChatRecordsQueryHandler(IValidator<ReadChatRecordsQuery> queryValidator, IChatHistoryRepository chatHistoryRepository)
        : base(queryValidator)
    {
        ChatHistoryRepository = Guard.Against.Null(chatHistoryRepository);
    }

    protected override Task<IEnumerable<string>> Execute(ReadChatRecordsQuery query)
    {
        query.StartDateTime.TryGetUtcDateTime(out UtcDateTime startDateTime);
        query.EndDateTime.TryGetUtcDateTime(out UtcDateTime endDateTime);

        return query.Granularity switch
        {
            Granularity.MinuteByMinute => ReadMinuteRecords(query, startDateTime, endDateTime),
            _ => ReadAggregatedRecords(query, startDateTime, endDateTime)
        };
    }

    private async Task<IEnumerable<string>> ReadMinuteRecords(ReadChatRecordsQuery query, UtcDateTime startDateTime, UtcDateTime endDateTime)
        => (await ChatHistoryRepository.ReadChatMinuteRecords(query.PageNumber, query.PageSize, startDateTime, endDateTime))
        .Select(r => r.ToText());

    private async Task<IEnumerable<string>> ReadAggregatedRecords(ReadChatRecordsQuery query, UtcDateTime startDateTime, UtcDateTime endDateTime)
        => (await ChatHistoryRepository.ReadChatAggregateRecords(query.Granularity, query.PageNumber, query.PageSize, startDateTime, endDateTime))
        .Select(r => r.ToText());
}
