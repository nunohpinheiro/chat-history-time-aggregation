using ChatHistory.ServiceApi.HealthChecks;
using Microsoft.OpenApi.Models;

namespace ChatHistory.ServiceApi.OpenApi;

internal static class OpenApiExtensions
{
    internal static IServiceCollection AddOpenApiSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Chat History API",
                    Version = "v1",
                    Description = "API to get chat history at varying levels of time-based aggregation, as well as adding chat events to the history"
                });

            options.DocumentHealthCheckEndpoints();
            options.EnableAnnotations();
        });

    internal static WebApplication UseOpenApiSwagger(this WebApplication app)
    {
        app.UseSwagger(o =>
        {
            o.RouteTemplate = "docs/{documentName}/open-api";
        });
        
        app.UseSwaggerUI(o =>
        {
            o.RoutePrefix = "docs/index";
            o.SwaggerEndpoint($"/docs/v1/open-api", "v1");
        });

        return app;
    }
}
