using ChatHistory.ServiceApi.HealthChecks;
using Microsoft.OpenApi.Models;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class OpenApiExtensions
{
    internal static IServiceCollection AddOpenApiSwagger(this IServiceCollection services)
        => services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                ApiMetadata.Version,
                new OpenApiInfo
                {
                    Title = ApiMetadata.Name,
                    Version = ApiMetadata.Version,
                    Description = ApiMetadata.Description
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
            o.SwaggerEndpoint($"/docs/{ApiMetadata.Version}/open-api", ApiMetadata.Version);
        });

        return app;
    }
}
