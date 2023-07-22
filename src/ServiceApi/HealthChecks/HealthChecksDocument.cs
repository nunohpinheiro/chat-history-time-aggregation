using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;

namespace ChatHistory.ServiceApi.HealthChecks;

internal class HealthChecksDocument : IDocumentFilter
{
    internal const string HealthCheckRoute = $"/v1/healthcheck";
    internal const string HealthCheckDeepRoute = $"{HealthCheckRoute}/deep";

    private static readonly OpenApiTag _healthcheckTag = new() { Name = "Health checks" };

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        MapHealthCheckEndpoint(swaggerDoc);
        MapHealthCheckDeepEndpoint(swaggerDoc);
    }

    private static void MapHealthCheckEndpoint(OpenApiDocument swaggerDoc)
    {
        OpenApiOperation operation = new();
        operation.Tags.Add(_healthcheckTag);

        var response = new OpenApiResponse();
        response.Content.Add(MediaTypeNames.Application.Json, new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
        operation.Responses.Add("200", response);

        OpenApiPathItem pathItem = new();
        pathItem.AddOperation(OperationType.Get, operation);

        swaggerDoc.Paths.Add(HealthCheckRoute, pathItem);
    }

    private static void MapHealthCheckDeepEndpoint(OpenApiDocument swaggerDoc)
    {
        OpenApiOperation operation = new();
        operation.Tags.Add(_healthcheckTag);

        var properties = new Dictionary<string, OpenApiSchema>
        {
            { "status", new OpenApiSchema { Type = "string" } },
            { "entries", new OpenApiSchema { Type = "array" } },
            { "totalDuration", new OpenApiSchema { Type = "string" } }
        };

        var response = new OpenApiResponse();
        response.Content.Add(MediaTypeNames.Application.Json, new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Type = "object",
                Properties = properties
            }
        });
        operation.Responses.Add("200", response);

        OpenApiPathItem pathItem = new();
        pathItem.AddOperation(OperationType.Get, operation);

        swaggerDoc.Paths.Add(HealthCheckDeepRoute, pathItem);
    }
}
