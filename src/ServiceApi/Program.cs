using ChatHistory.Domain.ChatRecords;
using ChatHistory.Infrastructure;
using ChatHistory.ServiceApi.AddChatHistory;
using ChatHistory.ServiceApi.ApiConfiguration;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddServiceApi()
    .AddInfrastructure(builder.Configuration);

var application = builder.Build();

application
    .UseServiceApi();

application.Run();
