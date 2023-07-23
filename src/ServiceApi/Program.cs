using ChatHistory.Infrastructure;
using ChatHistory.ServiceApi.ApiConfiguration;
using ChatHistory.ServiceApi.ChatRecords;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
    .Services
    .AddChatRecordsServices()
    .AddServiceApi()
    .AddInfrastructure(builder.Configuration);

var application = builder.Build();

application
    .UseServiceApi()
    .AddChatRecordsEndpoints();

application.Run();
