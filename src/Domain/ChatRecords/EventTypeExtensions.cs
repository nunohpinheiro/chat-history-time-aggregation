﻿using ChatHistory.Domain.ChatRecords;

namespace ChatHistory.Infrastructure.Persistence;

public static class EventTypeExtensions
{
    private const string EnterRoomDash = "enter-the-room";
    private const string LeaveRoomDash = "leave-the-room";
    private const string CommentDash = "comment";
    private const string HighFiveOtherUserDash = "high-five-another-user";

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
}