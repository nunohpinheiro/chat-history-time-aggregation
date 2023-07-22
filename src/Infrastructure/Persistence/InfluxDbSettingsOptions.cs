namespace ChatHistory.Infrastructure.Persistence;

internal class InfluxDbSettingsOptions
{
    internal const string SectionKey = "InfluxDbSettings";

    internal string Bucket { get; init; } = string.Empty;
    internal string Measurement { get; init; } = string.Empty;
    internal string Organization { get; init; } = string.Empty;
    internal string Token { get; init; } = string.Empty;
    internal string Url { get; init; } = string.Empty;
}
