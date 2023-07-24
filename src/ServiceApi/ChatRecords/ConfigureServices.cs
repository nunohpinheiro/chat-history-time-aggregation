using ChatHistory.Domain.ChatRecords;
using ChatHistory.Domain.ValueObjects;
using ChatHistory.ServiceApi.ApiConfiguration;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ChatHistory.ServiceApi.ChatRecords;

internal static class ConfigureServices
{
    internal static IServiceCollection AddChatRecordsServices(this IServiceCollection services)
        => services
        .AddScoped<CreateChatRecordCommandHandler>()
        .AddScoped<ReadChatRecordsQueryHandler>();

    internal static WebApplication AddChatRecordsEndpoints(this WebApplication application)
    {
        var chatHistory = application
            .MapGroup($"/{ApiMetadata.Version}/chat-records")
            .WithTags("Chat Records");

        chatHistory.MapGet("/", async (
            CancellationToken token,
            [FromServices] ReadChatRecordsQueryHandler queryHandler,
            [FromQuery] Granularity granularity,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 50,
            [FromQuery] string? startDateTime = "2021-07-16T00:00:00Z",
            [FromQuery] string? endDateTime = "2023-07-17T00:00:00Z")
            => (await queryHandler.Handle(
                new ReadChatRecordsQuery(
                    granularity,
                    (int)pageNumber!,
                    (int)pageSize!,
                    startDateTime!,
                    endDateTime!)))
                .Match(
                    eventsList => Results.Ok(eventsList),
                    failures => failures.ToValidationProblemDetails()))
            .AddReadChatRecordsOpenApi();

        chatHistory.MapPost("/", async (
            CancellationToken token,
            [FromServices] CreateChatRecordCommandHandler commandHandler,
            [FromBody] CreateChatRecordCommand createChatRecordCommand)
            => (await commandHandler
                .Handle(createChatRecordCommand, token))
                .Match(
                    success => Results.Created("/chat-records", createChatRecordCommand),
                    failures => failures.ToValidationProblemDetails()))
            .AddCreateChatRecordOpenApi();

        return application;
    }

    private static RouteHandlerBuilder AddCreateChatRecordOpenApi(this RouteHandlerBuilder routeHandlerBuilder)
        => routeHandlerBuilder
        .WithDescription("Create a new chat record")
        .WithOpenApi(generatedOperation =>
        {
            var requestBodyDescription =
                $$"""
                ```json
                {
                  "eventType": "{{EventType.EnterRoom.ToDashedEvent()}}", // Or {{EventType.LeaveRoom.ToDashedEvent()}}, {{EventType.Comment.ToDashedEvent()}}, {{EventType.HighFiveOtherUser.ToDashedEvent()}}
                  "timestamp": "2023-07-17T00:00:00Z", // When the event occurs, must be in format {{UtcDateTime.ValidFormat}}
                  "user": "Mrs. Sample User", // Who performs the event, must respect the regex {{Username.UserFormatRegex}}
                  "commentText": "Sample comment", // Used in {{EventType.Comment.ToDashedEvent()}} event types (it is the comment itself)
                  "highFivedPerson": "Mr. HighFive Receiver" // Used in {{EventType.HighFiveOtherUser.ToDashedEvent()}} event types (the person that receives the high-five), must respect the regex {{Username.UserFormatRegex}}
                }
                """;
            generatedOperation.RequestBody.Description = requestBodyDescription;
            return generatedOperation;
        })
        .Accepts<CreateChatRecordCommand>(MediaTypeNames.Application.Json)
        .Produces<CreateChatRecordCommand>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status500InternalServerError);

    private static RouteHandlerBuilder AddReadChatRecordsOpenApi(this RouteHandlerBuilder routeHandlerBuilder)
        => routeHandlerBuilder
        .WithDescription("Query the chat records collection according to set parameters")
        .WithOpenApi(generatedOperation =>
        {
            var startDateTime = generatedOperation.Parameters[3];
            startDateTime.Description = "Start of the search query, must be in format \"yyyy-MM-ddTHH:mm:ssZ\"";
            var endDateTime = generatedOperation.Parameters[4];
            endDateTime.Description = "End of the search query, must be in format \"yyyy-MM-ddTHH:mm:ssZ\"";
            return generatedOperation;
        })
        .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
}
