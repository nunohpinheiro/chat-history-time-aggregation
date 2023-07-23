using System.Reflection;

namespace ChatHistory.ServiceApi.IntegrationTests.Fixtures;

public abstract class SnapshotTestsBase
{
    protected readonly string SnapshotFilesPath =
        Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "test-snapshots");
}
