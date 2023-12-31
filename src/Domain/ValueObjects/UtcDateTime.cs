﻿using System.Globalization;
using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class UtcDateTime : ValueOf<DateTime, UtcDateTime>
{
    public const string ValidFormat = "yyyy-MM-ddTHH:mm:ssZ";

    protected override void Validate()
    {
        if (TryValidate() is false)
            Value = Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(Value, DateTimeKind.Utc) : Value.ToUniversalTime();
    }

    protected override bool TryValidate()
        => Value.Kind == DateTimeKind.Utc;

    public static implicit operator DateTime(UtcDateTime s) => s.Value;

    public static implicit operator UtcDateTime(DateTime s) => From(s);
}

public static class UtcDateTimeExtensions
{
    public static bool TryGetUtcDateTime(this string value, out UtcDateTime utcDateTime)
    {
        utcDateTime = null!;

        var isValid =
            DateTime.TryParseExact(
                value,
                UtcDateTime.ValidFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dateTime);

        if (isValid)
            utcDateTime = dateTime;

        return isValid;
    }
}
