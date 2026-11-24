namespace TimeSeries.Core.Exceptions;

/// <summary>
/// Thrown when input breaks one or more rules. Errors are grouped by the field they concern.
/// </summary>
public class RecordValidationException(IReadOnlyDictionary<string, string[]> errors)
    : Exception(string.Join(" ", errors.Values.SelectMany(messages => messages)))
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
