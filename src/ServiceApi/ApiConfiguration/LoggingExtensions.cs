using Serilog;

namespace ChatHistory.ServiceApi.ApiConfiguration;

internal static class LoggingExtensions
{
    internal static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Host.UseSerilog(logger);

        return builder;
    }
}
