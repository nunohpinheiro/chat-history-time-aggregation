namespace ChatHistory.Infrastructure.Persistence;

internal class InfluxDbSettingsOptions
{
    internal const string SectionKey = "InfluxDbSettings";

    public string Bucket { get; init; } = string.Empty;
    public string Measurement { get; init; } = string.Empty;
    public string Organization { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
