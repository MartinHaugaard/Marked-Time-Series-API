namespace TimeSeries.Core.Exceptions;

/// <summary>
/// Collects validation errors per field, so all violations can be reported at once.
/// </summary>
internal sealed class ValidationErrors
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var messages))
            _errors[field] = messages = new List<string>();

        messages.Add(message);
    }

    public void ThrowIfAny()
    {
        if (_errors.Count > 0)
            throw new RecordValidationException(_errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));
    }
}
