using ChatHistory.Domain.ChatRecords;
using InfluxDB.Client.Core.Flux.Domain;

namespace ChatHistory.Infrastructure.Persistence;

internal static class ChatRecordFactoryExtensions
{
    internal static List<ChatAggregateRecord> ToChatAggregateRecords(this List<FluxTable> tables, Granularity granularity)
        => tables.SelectMany(table =>
            table.Records.Select(record =>
                new ChatAggregateRecord
                {
                    Count = Convert.ToInt32(record.GetValueByKey("_value")),
                    Day = record.GetOptionalInt("day"),
                    DashedEventType = record.GetValueByKey("event-type").ToString()!,
                    Granularity = granularity,
                    HourFormat = record.GetValueByKey("hour-format")?.ToString()!,
                    Month = record.GetOptionalInt("month"),
                    Year = Convert.ToInt32(record.GetValueByKey("year"))
                })).ToList();

    internal static List<ChatMinuteRecord> ToChatMinuteRecords(this List<FluxTable> tables)
        => tables.SelectMany(table =>
            table.Records.Select(record =>
                new ChatMinuteRecord
                {
                    MinuteEvent = record.GetValueByKey("_value").ToString()!,
                    MinuteFormat = record.GetValueByKey("minute-format").ToString()!,
                    Timestamp = record.GetTimeInDateTime()!,
                    User = record.GetValueByKey("user").ToString()!,
                })).ToList();

    private static int? GetOptionalInt(this FluxRecord record, string key)
        => record.GetValueByKey(key) is null ? null : Convert.ToInt32(record.GetValueByKey(key));
}
