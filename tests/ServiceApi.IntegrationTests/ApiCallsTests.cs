using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using ChatHistory.ServiceApi.ChatRecords;
using ChatHistory.ServiceApi.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace ChatHistory.ServiceApi.IntegrationTests;

[UsesVerify]
public class ApiCallsTests : SnapshotTestsBase, IClassFixture<ChatHistoryInfluxDbFixture>
{
    private const string ChatRecordsUri = "/v1/chat-records";
    private readonly HttpClient TestServerClient;

    public ApiCallsTests()
    {
        TestServerClient = ApiTestsServerFactory
            .CreateWebAppFactory()
            .CreateClient();
    }

    [Fact]
    public async Task CallDocs_GetOpenApi_SnapshotIsMatched()
    {
        var response = await TestServerClient.GetAsync("/docs/v1/open-api");
        var openApi = await response.Content.ReadAsStringAsync();
        await Verify(openApi)
            .UseFileName($"{nameof(ApiCallsTests)}_{nameof(CallDocs_GetOpenApi_SnapshotIsMatched)}")
            .UseDirectory(SnapshotFilesPath);
    }

    [Fact]
    public async Task GetHealthCheck_ResponseIs200Ok()
        => (await TestServerClient.GetAsync("/v1/healthcheck"))
        .StatusCode
        .Should().Be(HttpStatusCode.OK);

    [Fact]
    public async Task GetHealthCheckDeep_ResponseIs200Ok()
        => (await TestServerClient.GetAsync("/v1/healthcheck/deep"))
        .StatusCode
        .Should().Be(HttpStatusCode.OK);

    [Theory]
    [InlineData(Granularity.MinuteByMinute, 2023, 7, 23)]
    [InlineData(Granularity.Hourly, 2023, 7, 21)]
    [InlineData(Granularity.Daily, 2023, 7, 19)]
    [InlineData(Granularity.Monthly, 2023, 5, 23)]
    [InlineData(Granularity.Yearly, 2022, 5, 23)]
    public async Task PostChatRecords_GetChatRecords_SnapshotIsMatched(Granularity granularity, int year, int month, int day)
    {
        // Arrange
        var timestamp = new DateTime(year, month, day, 13, 51, 0);
        
        var commandsList = GetCommands(timestamp);
        
        var readQuery = $"?granularity={granularity}" +
            $"&pageNumber=1&pageSize=50" +
            $"&startDateTime={timestamp.AddHours(-1).ToString(UtcDateTime.ValidFormat)}" +
            $"&endDateTime={timestamp.AddHours(2).ToString(UtcDateTime.ValidFormat)}";

        // Act/Assert
        foreach (var command in commandsList)
        {
            (await TestServerClient.PostAsJsonAsync(ChatRecordsUri, command))
                .StatusCode
                .Should().Be(HttpStatusCode.Created);
        }

        var response = await TestServerClient.GetAsync(ChatRecordsUri + readQuery);
        var minuteResults = await response.Content.ReadAsStringAsync();

        response.StatusCode
            .Should().Be(HttpStatusCode.OK);

        await Verify(minuteResults.Replace("\",\"", "\",\n\""))
            .UseFileName($"{nameof(ApiCallsTests)}_{nameof(PostChatRecords_GetChatRecords_SnapshotIsMatched)}_{granularity}_{year}{month}{day}")
            .UseDirectory(SnapshotFilesPath);
    }

    private static List<CreateChatRecordCommand> GetCommands(DateTime timestamp)
    {
        var user1 = "User1";
        var user2 = "User2";
        return new List<CreateChatRecordCommand>()
        {
            new(EventType.Comment.ToDashedEvent(), timestamp.ToString(UtcDateTime.ValidFormat), user1, "sample comment 1", null!),
            new(EventType.Comment.ToDashedEvent(), timestamp.AddMinutes(5).ToString(UtcDateTime.ValidFormat), user2, "sample comment 2", null!),
            new(EventType.EnterRoom.ToDashedEvent(), timestamp.AddMinutes(10).ToString(UtcDateTime.ValidFormat), user1, null!, null!),
            new(EventType.EnterRoom.ToDashedEvent(), timestamp.AddMinutes(15).ToString(UtcDateTime.ValidFormat), user2, null!, null!),
            new(EventType.HighFiveOtherUser.ToDashedEvent(), timestamp.AddMinutes(20).ToString(UtcDateTime.ValidFormat), user1, null!, "Mr. User"),
            new(EventType.HighFiveOtherUser.ToDashedEvent(), timestamp.AddMinutes(22).ToString(UtcDateTime.ValidFormat), user2, null!, "Mrs. User"),
            new(EventType.HighFiveOtherUser.ToDashedEvent(), timestamp.AddMinutes(25).ToString(UtcDateTime.ValidFormat), user2, null!, "Mrs. Another User"),
            new(EventType.LeaveRoom.ToDashedEvent(), timestamp.AddMinutes(30).ToString(UtcDateTime.ValidFormat), user1, null!, null!),
            new(EventType.LeaveRoom.ToDashedEvent(), timestamp.AddMinutes(35).ToString(UtcDateTime.ValidFormat), user2, null!, null!)
        };
    }
}
