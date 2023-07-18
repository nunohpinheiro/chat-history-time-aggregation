using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;

namespace ChatHistory.Infrastructure;

internal class ChatHistoryInfluxDbRepository
{
    private readonly string _token;

    public ChatHistoryInfluxDbRepository()
    {
        _token = "myadmintoken";
    }

    public async Task<IEnumerable<ChatRecord>> GetMinuteChatEvents()
    {
        var client = new InfluxDBClient("http://localhost:8086", _token);

        var queryApi = client.GetQueryApi();
        var tables = await queryApi.QueryAsync(
            "from(bucket:\"mybucket\")" +
            "|> range(start: 0)" +
            "|> drop(columns: [\"_measurement\", \"_start\", \"_stop\", \"_field\"])" +
            "|> group()" +
            "|> sort(columns: [\"_time\"])",
            "myorg");

        var chatHist = GetChatHistory(tables);
        return chatHist;
    }

    private static List<ChatRecord> GetChatHistory(List<FluxTable> tables)
        => tables.SelectMany(table =>
            table.Records.Select(record =>
                new ChatRecord
                {
                    Day = Convert.ToInt32(record.GetValueByKey("day")),
                    EventType = record.GetValueByKey("event-type").ToString(),
                    HourFormat = record.GetValueByKey("hour-format").ToString(),
                    MinuteEvent = record.GetValueByKey("_value").ToString(),
                    MinuteFormat = record.GetValueByKey("minute-format").ToString(),
                    Month = Convert.ToInt32(record.GetValueByKey("month")),
                    User = record.GetValueByKey("user").ToString(),
                    Timestamp = (DateTime)record.GetTimeInDateTime()!,
                    Year = Convert.ToInt32(record.GetValueByKey("year"))
                })).ToList();
}
