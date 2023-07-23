using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ChatHistory.ServiceApi.IntegrationTests.Fixtures;

internal class ApiTestsServerFactory : WebApplicationFactory<Program>
{
    internal static WebApplicationFactory<Program> CreateWebAppFactory()
        => new ApiTestsServerFactory()
        .WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((whb, cfgb) =>
            {
                cfgb.AddInMemoryCollection(new List<KeyValuePair<string, string?>>(1)
                {
                    new("InfluxDbSettings:Bucket", ChatHistoryInfluxDbFixture.TestBucketName),
                    new("InfluxDbSettings:Measurement", "chat-history"),
                    new("InfluxDbSettings:Organization", ChatHistoryInfluxDbFixture.TestOrganizationName),
                    new("InfluxDbSettings:Token", "myadmintoken"),
                    new("InfluxDbSettings:Url", "http://localhost:8086"),
                });
            });
        });
}
