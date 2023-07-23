using System.Text.RegularExpressions;
using ValueOf;

namespace ChatHistory.Domain.ValueObjects;

public class Username : ValueOf<string, Username>
{
    public const string UserFormatRegex = "^[ A-Za-z0-9_@.-]*$";
    
    protected override void Validate()
    {
        if (!TryValidate())
            throw new ArgumentException($"The string does not follow the required format '{UserFormatRegex}'.");
    }

    protected override bool TryValidate()
        => !string.IsNullOrWhiteSpace(Value)
        && Regex.Match(Value, UserFormatRegex, RegexOptions.Compiled, TimeSpan.FromSeconds(1)).Success;

    public static implicit operator string(Username s) => s.Value;

    public static implicit operator Username(string s) => From(s);
}
