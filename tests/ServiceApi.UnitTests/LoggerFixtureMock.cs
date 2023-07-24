using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;

namespace ChatHistory.ServiceApi.UnitTests;

internal class LoggerFixtureMock<TCategoryName> : ILogger<TCategoryName>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        return;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new CancellationDisposable();
    }
}
