using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChatHistory.ServiceApi.HealthChecks;

internal static class HealthChecksExtensions
{
    internal static void DocumentHealthCheckEndpoints(this SwaggerGenOptions options)
        => options.DocumentFilter<HealthChecksDocument>();

    internal static WebApplication AddHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks(
            $"/v1/healthcheck",
            new HealthCheckOptions
            {
                Predicate = _ => false
            });

        app.MapHealthChecks(
            $"/v1/healthcheck/deep",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResultStatusCodes = new Dictionary<HealthStatus, int> {
                        { HealthStatus.Healthy, 200 },
                        { HealthStatus.Degraded, 200 },
                        { HealthStatus.Unhealthy, 200 }
                    },
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }
}
