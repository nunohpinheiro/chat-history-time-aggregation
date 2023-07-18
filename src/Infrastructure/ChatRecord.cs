namespace ChatHistory.Infrastructure;

public class ChatRecord
{
    public string? EventType { get; set; }
    public string? MinuteFormat { get; set; }
    public string? HourFormat { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? User { get; set; }
    public string? MinuteEvent { get; set; }
    public DateTime Timestamp { get; set; }
}
