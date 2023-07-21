using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class UtcDateTime : ValueOf<DateTime, UtcDateTime>
{
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
