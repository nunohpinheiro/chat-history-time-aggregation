using ChatHistory.ServiceApi.HealthChecks;
using ToolPack.Exceptions.Web.Extensions;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class ConfigureServices
{
    internal static IServiceCollection AddServiceApi(this IServiceCollection services)
        => services
        .AddToolPackExceptions()
        .AddEndpointsApiExplorer()
        .AddCustomJsonSerialization()
        .AddOpenApiSwagger();

    internal static void UseServiceApi(this WebApplication application)
        => application
        .UseOpenApiSwagger()
        .AddHealthCheckEndpoints()
        .UseHttpsRedirection()
        .UseToolPackExceptionsMiddleware();
}
