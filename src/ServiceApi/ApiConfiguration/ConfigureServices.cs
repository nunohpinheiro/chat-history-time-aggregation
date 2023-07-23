using ChatHistory.ServiceApi.HealthChecks;
using FluentValidation;
using ToolPack.Exceptions.Web.Extensions;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class ConfigureServices
{
    internal static IServiceCollection AddServiceApi(this IServiceCollection services)
        => services
        .AddToolPackExceptions()
        .AddEndpointsApiExplorer()
        .AddCustomJsonSerialization()
        .AddValidatorsFromAssemblyContaining<Program>()
        .AddOpenApiSwagger();

    internal static WebApplication UseServiceApi(this WebApplication application)
    {
        application
            .UseOpenApiSwagger()
            .AddHealthCheckEndpoints()
            .UseHttpsRedirection()
            .UseToolPackExceptionsMiddleware();

        return application;
    }
}
