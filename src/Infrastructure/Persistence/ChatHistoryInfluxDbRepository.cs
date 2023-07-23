using Ardalis.GuardClauses;
using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;

namespace ChatHistory.Infrastructure.Persistence;

internal class ChatHistoryInfluxDbRepository : IChatHistoryRepository
{
    private readonly string _bucket;
    private readonly string _measurement;
    private readonly string _organization;
    private readonly string _token;
    private readonly string _url;

    public ChatHistoryInfluxDbRepository(IOptions<InfluxDbSettingsOptions> influxDbOptionsAccessor)
    {
        var influxDbOptions = Guard.Against.Null(influxDbOptionsAccessor.Value);

        _bucket = Guard.Against.NullOrWhiteSpace(influxDbOptions.Bucket);
        _measurement = Guard.Against.NullOrWhiteSpace(influxDbOptions.Measurement);
        _organization = Guard.Against.NullOrWhiteSpace(influxDbOptions.Organization);
        _token = Guard.Against.NullOrWhiteSpace(influxDbOptions.Token);
        _url = Guard.Against.NullOrWhiteSpace(influxDbOptions.Url);
    }

    public async Task AddChatHistoryEvent(ChatRecordEvent chatHistoryEvent, CancellationToken cancellationToken = default)
    {
        using var influxDbClient = new InfluxDBClient(_url, _token);
        var writeApi = influxDbClient.GetWriteApiAsync();

        var point = PointData
            .Measurement(_measurement)
            .Tag("event-type",
                chatHistoryEvent.EventType is EventType.HighFiveOtherUser
                ? chatHistoryEvent.EventType.ToDetailHighFiveEvent(chatHistoryEvent.User, chatHistoryEvent.HighFivedPerson)
                : chatHistoryEvent.EventType.ToDashedEvent())
            .Tag("minute-format", chatHistoryEvent.MinuteFormat)
            .Tag("hour-format", chatHistoryEvent.HourFormat)
            .Tag("day", ((int)chatHistoryEvent.Day).ToString())
            .Tag("month", ((int)chatHistoryEvent.Month).ToString())
            .Tag("year", ((int)chatHistoryEvent.Year).ToString())
            .Tag("user", chatHistoryEvent.User)
            .Field("minute-event", chatHistoryEvent.MinuteEvent)
            .Timestamp(chatHistoryEvent.Timestamp, WritePrecision.S);

        await writeApi.WritePointAsync(point, _bucket, _organization, cancellationToken);
    }

    public async Task<List<ChatAggregateRecord>> ReadChatAggregateRecords(
        Granularity granularity, PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange, CancellationToken cancellationToken = default)
    {
        using var influxDbClient = new InfluxDBClient(_url, _token);

        var tables = await influxDbClient.GetQueryApi().QueryAsync(
            GetBaseQuery(startRange, endRange) +
            "|> drop(columns: [\"_measurement\", \"_start\", \"_stop\"])" +
            $"|> group(columns: [{GetGroupColumns(granularity)}])" +
            "|> count()" +
            "|> group()" +
            $"|> sort(columns: [{GetGroupColumns(granularity)}])" +
            GetPageQuery(pageNumber, pageSize),
            _organization,
            cancellationToken);

        var aggregates = tables.ToChatAggregateRecords(granularity);
        var high5Aggregates = aggregates.Where(a => a.EventType == EventType.HighFiveOtherUser).ToList();

        return high5Aggregates.Any()
            ? AggregateWithGroupedHighFives(aggregates, high5Aggregates)
            : aggregates;
    }

    public async Task<List<ChatMinuteRecord>> ReadChatMinuteRecords(
        PositiveInt pageNumber, PositiveInt pageSize, UtcDateTime startRange, UtcDateTime endRange, CancellationToken cancellationToken = default)
    {
        using var influxDbClient = new InfluxDBClient(_url, _token);

        var tables = await influxDbClient.GetQueryApi().QueryAsync(
            GetBaseQuery(startRange, endRange) +
            "|> drop(columns: [\"_measurement\", \"_start\", \"_stop\", \"_field\"])" +
            "|> group()" +
            "|> sort(columns: [\"_time\"])" +
            GetPageQuery(pageNumber, pageSize),
            _organization,
            cancellationToken);

        return tables.ToChatMinuteRecords();
    }

    private static string GetGroupColumns(Granularity granularity)
        => granularity switch
        {
            Granularity.Hourly => "\"year\", \"month\", \"day\", \"hour-format\", \"event-type\"",
            Granularity.Daily => "\"year\", \"month\", \"day\", \"event-type\"",
            Granularity.Monthly => "\"year\", \"month\", \"event-type\"",
            Granularity.Yearly => "\"year\", \"event-type\"",
            _ => throw new InvalidOperationException($"Granularity value of {granularity} is not supported")
        };

    private static string GetPageQuery(int pageNumber, int pageSize)
        => $"|> limit(n:{pageSize},offset:{GetPageOffset(pageNumber, pageSize)})";

    private static int GetPageOffset(int pageNumber, int pageSize)
        => (pageNumber - 1) * pageSize;

    private static string GetFormatDateTime(UtcDateTime datetime)
        => datetime.Value.ToString("u").Replace(" ", "T");

    private string GetBaseQuery(UtcDateTime startRange, UtcDateTime endRange)
        => $"from(bucket:\"{_bucket}\")" +
        $"|> range(start: {GetFormatDateTime(startRange)}, stop: {GetFormatDateTime(endRange)})" +
        $"|> filter(fn: (r) => r[\"_measurement\"] == \"{_measurement}\")";

    private static List<ChatAggregateRecord> AggregateWithGroupedHighFives(
        List<ChatAggregateRecord> aggregates, List<ChatAggregateRecord> high5Aggregates)
    {
        var otherAggregates = aggregates.Except(high5Aggregates).ToList();
        var high5Groups = high5Aggregates.GroupBy(a => a.Order).ToList();

        Parallel.ForEach(high5Groups,
            currentGroup =>
            {
                if (currentGroup.Any() is false)
                    return;

                var high5GiversCount = currentGroup.DistinctBy(h => h.HighFiveUsers.highFiveGiver).Count();
                var high5ReceiversCount = currentGroup.DistinctBy(h => h.HighFiveUsers.highFiveReceiver).Count();

                var groupAggregate = new ChatAggregateRecord
                {
                    Count = high5GiversCount,
                    DashedEventType = EventType.HighFiveOtherUser.ToDashedEvent(),
                    Day = currentGroup.First().Day,
                    Granularity = currentGroup.First().Granularity,
                    HourFormat = currentGroup.First().HourFormat,
                    Month = currentGroup.First().Month,
                    Year = currentGroup.First().Year
                };

                groupAggregate.SetHighFiveReceiversCount(high5ReceiversCount);

                otherAggregates.Add(groupAggregate);
            });

        return otherAggregates.OrderBy(a => a.Order).ToList();
    }
}
