using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeSeries.Api.Binding;

/// <summary>
/// Rejects DateTimeOffset values in request bodies that don't state their offset.
/// </summary>
internal sealed class ExplicitOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String || !ExplicitOffset.TryParse(reader.GetString(), out var value))
            throw new JsonException(ExplicitOffset.ErrorMessage);

        return value;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
