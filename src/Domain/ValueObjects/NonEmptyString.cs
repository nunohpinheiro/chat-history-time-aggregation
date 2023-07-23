using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class NonEmptyString : ValueOf<string, NonEmptyString>
{
    protected override void Validate()
    {
        if (!TryValidate())
            throw new ArgumentException($"The string must not be null, empty or white space(s).");
    }

    protected override bool TryValidate()
        => !string.IsNullOrWhiteSpace(Value);

    public static implicit operator string(NonEmptyString s) => s.Value;

    public static implicit operator NonEmptyString(string s) => From(s);
}
