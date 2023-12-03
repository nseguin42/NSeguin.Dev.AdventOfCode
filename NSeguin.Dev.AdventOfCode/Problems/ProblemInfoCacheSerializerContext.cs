using System.Text.Json;
using System.Text.Json.Serialization;

namespace NSeguin.Dev.AdventOfCode;

[JsonSerializable(typeof(ProblemId))]
[JsonSerializable(typeof(ProblemInfo))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSourceGenerationOptions(
    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [typeof(ProblemIdJsonConverter)])]
internal partial class ProblemInfoCacheSerializerContext : JsonSerializerContext;
