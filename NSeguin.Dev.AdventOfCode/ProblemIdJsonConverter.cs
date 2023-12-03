using System.Text.Json;
using System.Text.Json.Serialization;

namespace NSeguin.Dev.AdventOfCode;

public class ProblemIdJsonConverter : JsonConverter<ProblemId>
{
    public override ProblemId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        string[] parts = reader.GetString()!.Split('-');
        return new ProblemId(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public override void Write(
        Utf8JsonWriter writer,
        ProblemId value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.Year}-{value.Day}");
    }
}
