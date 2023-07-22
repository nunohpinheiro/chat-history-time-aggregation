using ChatHistory.Domain;
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
            .AddScoped<IChatHistoryRepository, ChatHistoryInfluxDbRepository>();

        return services;
    }
}
