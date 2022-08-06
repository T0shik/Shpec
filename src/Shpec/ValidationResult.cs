using System.Collections.Generic;

namespace Shpec.Validation;

public interface IValidatable
{
    ValidationResult Valid();
}

public record struct ValidationResult(IReadOnlyCollection<ValidationError> Errors)
{
    public bool Success => Errors.Count == 0;

    public static implicit operator bool(ValidationResult v) => v.Success;
}

public record struct ValidationError(string Key, object Value, string Error);