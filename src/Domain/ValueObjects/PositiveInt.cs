using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class PositiveInt : ValueOf<int, PositiveInt>
{
    protected override void Validate()
    {
        if (!TryValidate())
            throw new ArgumentException($"The value {Value} is not a positive integer");
    }

    protected override bool TryValidate()
        => Value > 0;

    public static implicit operator int(PositiveInt s) => s.Value;

    public static implicit operator PositiveInt(int s) => From(s);
}
