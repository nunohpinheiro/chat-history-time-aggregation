using ChatHistory.Domain.ChatRecords;
using ChatHistory.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatHistory.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<InfluxDbSettingsOptions>(configuration.GetSection(InfluxDbSettingsOptions.SectionKey))
            .AddScoped<IChatHistoryRepository, ChatHistoryInfluxDbRepository>()
            .AddHealthChecks()
            .AddInfluxDB(
                configuration.GetValue<string>($"{InfluxDbSettingsOptions.SectionKey}:Url")!,
                configuration.GetValue<string>($"{InfluxDbSettingsOptions.SectionKey}:Token")!,
                name: "chat-history-influxdb-repository");

        return services;
    }
}
