using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Exceptions;

namespace ChatHistory.Infrastructure.IntegrationTests;

public class ChatHistoryInfluxDbRepositoryTestsFixture : IAsyncLifetime
{
    public const string TestBucketName = "repository-tests-bucket";
    public const string TestOrganizationName = "repository-tests-org";

    private const string InfluxDbToken = "myadmintoken";
    private const string InfluxDbUrl = "http://localhost:8086";

    public async Task InitializeAsync()
    {
        var testOrgId = await GetOrCreateTestOrganization();
        await EnsureCreateTestBucket(testOrgId);
    }

    public async Task DisposeAsync()
    {
        await EnsureDeleteTestBucket();
        await EnsureDeleteTestOrganization();
    }

    private static async Task<string> GetOrCreateTestOrganization()
    {
        using var influxDbClient = new InfluxDBClient(InfluxDbUrl, InfluxDbToken);
        var orgsApi = influxDbClient.GetOrganizationsApi();

        Organization? testOrganization;
        try
        {
            testOrganization = (await orgsApi.FindOrganizationsAsync(org: TestOrganizationName)).SingleOrDefault();
        }
        catch (NotFoundException)
        {
            testOrganization = await orgsApi.CreateOrganizationAsync(TestOrganizationName);
        };

        return testOrganization!.Id;
    }

    private static async Task EnsureCreateTestBucket(string organizationId)
    {
        using var influxDbClient = new InfluxDBClient(InfluxDbUrl, InfluxDbToken);
        var bucketsApi = influxDbClient.GetBucketsApi();

        if ((await bucketsApi.FindBucketByNameAsync(TestBucketName)) is null)
            await bucketsApi.CreateBucketAsync(new Bucket(
                name: TestBucketName,
                orgID: organizationId,
                retentionRules: new List<BucketRetentionRules>(1)
                {
                    new BucketRetentionRules(everySeconds: 86400) // 1 day
                }));
    }

    private static async Task EnsureDeleteTestBucket()
    {
        using var influxDbClient = new InfluxDBClient(InfluxDbUrl, InfluxDbToken);
        var bucketsApi = influxDbClient.GetBucketsApi();

        var testBucket = await bucketsApi.FindBucketByNameAsync(TestBucketName);
        if (testBucket is not null)
            await bucketsApi.DeleteBucketAsync(testBucket.Id);
    }

    private static async Task EnsureDeleteTestOrganization()
    {
        using var influxDbClient = new InfluxDBClient(InfluxDbUrl, InfluxDbToken);
        var orgsApi = influxDbClient.GetOrganizationsApi();

        Organization? testOrganization;
        try
        {
            testOrganization = (await orgsApi.FindOrganizationsAsync(org: TestOrganizationName)).SingleOrDefault();
        }
        catch (NotFoundException)
        {
            testOrganization = null;
        };

        if (testOrganization is not null)
            await orgsApi.DeleteOrganizationAsync(testOrganization.Id);
    }
}
