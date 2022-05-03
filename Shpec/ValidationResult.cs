using System.Collections.Immutable;

namespace Shpec;

public record struct ValidationResult(ImmutableArray<ValidationError> Errors)
{
    public bool Success => Errors.Length == 0;

    public static implicit operator bool(ValidationResult v) => v.Success;
}

public record struct ValidationError(string Key, object Value, string Error);