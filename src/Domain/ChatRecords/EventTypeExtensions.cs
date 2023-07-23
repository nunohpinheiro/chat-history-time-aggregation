namespace ChatHistory.Domain.ChatRecords;

public static class EventTypeExtensions
{
    private const string EnterRoomDash = "enter-the-room";
    private const string LeaveRoomDash = "leave-the-room";
    private const string CommentDash = "comment";
    private const string HighFiveOtherUserDash = "high-five-another-user";

    private const string HighFiveEventSeparator = "#";

    private static readonly Dictionary<EventType, string> EventToDashDictionary = new()
    {
        { EventType.EnterRoom, EnterRoomDash },
        { EventType.LeaveRoom, LeaveRoomDash },
        { EventType.Comment, CommentDash },
        { EventType.HighFiveOtherUser, HighFiveOtherUserDash },
    };

    private static readonly Dictionary<string, EventType> DashToEventDictionary = new()
    {
        { EnterRoomDash, EventType.EnterRoom },
        { LeaveRoomDash, EventType.LeaveRoom },
        { CommentDash, EventType.Comment },
        { HighFiveOtherUserDash, EventType.HighFiveOtherUser },
    };

    public static string ToDashedEvent(this EventType eventType) => EventToDashDictionary[eventType];

    public static EventType ToEventType(this string dashedEventType) => DashToEventDictionary[dashedEventType];

    public static bool TryGetEventType(this string dashedEventType, out EventType eventType)
        => DashToEventDictionary.TryGetValue(dashedEventType, out eventType);

    public static string ToDetailHighFiveEvent(this EventType eventType, string highFiveGiver, string highFiveReceiver)
        => eventType is EventType.HighFiveOtherUser
        ? $"{eventType.ToDashedEvent()}{HighFiveEventSeparator}{highFiveGiver}{HighFiveEventSeparator}{highFiveReceiver}"
        : throw new InvalidOperationException($"Only events of type '{EventType.HighFiveOtherUser}' can execute '{nameof(ToDetailHighFiveEvent)}' operations. This type was '{eventType}'");

    public static (EventType eventType, string highFiveGiver, string highFiveReceiver) GetHighFiveEventDetails(this string detailHighFiveEventType)
    {
        var eventDetails = detailHighFiveEventType.Split(HighFiveEventSeparator);

        if (eventDetails.Length != 3)
            throw new InvalidOperationException($"Only events of type '{EventType.HighFiveOtherUser}' can execute '{nameof(GetHighFiveEventDetails)}' operations. This type was '{detailHighFiveEventType}'");

        return (eventDetails[0].ToEventType(), eventDetails[1], eventDetails[2]);
    }
}
