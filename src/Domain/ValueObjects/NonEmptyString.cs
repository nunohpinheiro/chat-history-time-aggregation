using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class NonEmptyString : ValueOf<string, NonEmptyString>
{
    protected override void Validate()
    {
        if (!TryValidate())
            throw new ArgumentException($"The value {Value} is null, empty string or white space(s).");
    }

    protected override bool TryValidate()
        => !string.IsNullOrWhiteSpace(Value);

    public static implicit operator string(NonEmptyString s) => s.Value;

    public static implicit operator NonEmptyString(string s) => From(s);
}
