using ChatHistory.Infrastructure;
using ChatHistory.ServiceApi.HealthChecks;
using ChatHistory.ServiceApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddOpenApiSwagger()
    .AddInfrastructure(builder.Configuration);

var application = builder.Build();

application
    .UseOpenApiSwagger()
    .AddHealthCheckEndpoints()
    .UseHttpsRedirection();

application.Run();
