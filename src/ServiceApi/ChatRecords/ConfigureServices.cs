using ChatHistory.Domain.ChatRecords;
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
            .WithDescription("Query the chat records collection according to set parameters")
            .WithOpenApi()
            .Produces<IEnumerable<string>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        chatHistory.MapPost("/", async (
            CancellationToken token,
            [FromServices] CreateChatRecordCommandHandler commandHandler,
            [FromBody] CreateChatRecordCommand createChatRecordCommand)
            => (await commandHandler
                .Handle(createChatRecordCommand, token))
                .Match(
                    success => Results.Created("/chat-records", createChatRecordCommand),
                    failures => failures.ToValidationProblemDetails()))
            .WithDescription("Create a new chat record")
            .WithOpenApi()
            .Accepts<CreateChatRecordCommand>(MediaTypeNames.Application.Json)
            .Produces<IEnumerable<string>>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return application;
    }
}
