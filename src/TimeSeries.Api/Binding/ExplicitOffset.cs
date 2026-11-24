using System.Globalization;

namespace TimeSeries.Api.Binding;

/// <summary>
/// Parses timestamps that must state their offset, e.g. "2026-11-21T12:00:00Z".
/// DateTimeOffset.Parse silently assumes the server's local offset when none is given,
/// which would make the same request valid on one server and invalid on another.
/// </summary>
internal static class ExplicitOffset
{
    public const string ErrorMessage = "Timestamp must include an offset, e.g. '2026-11-21T12:00:00Z'.";

    public static bool TryParse(string? text, out DateTimeOffset value)
    {
        value = default;

        // RoundtripKind leaves the kind Unspecified when the text has no offset or 'Z'.
        return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime)
            && dateTime.Kind != DateTimeKind.Unspecified
            && DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
    }
}
