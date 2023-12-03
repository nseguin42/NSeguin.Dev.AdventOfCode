using System.Text.Json;

using NSeguin.Dev.AdventOfCode.Utilities;

namespace NSeguin.Dev.AdventOfCode;

internal class ProblemInfoCache : JsonFileCache
{
    public ProblemInfoCache(JsonFileCacheRegistry registry, string sessionIdHash, string fileName)
        : base(registry, fileName, ProblemInfoCacheSerializerContext.Default.Options)
    {
        SessionIdHash = sessionIdHash;
    }

    public string SessionIdHash { get; }

    public async ValueTask<ProblemInfo?> GetProblemInfoAsync(ProblemId id)
    {
        await EnsureLoadedAsync().ConfigureAwait(false);
        return Values.TryGetValue(id.ToString(), out JsonElement value)
            ? value.Deserialize<ProblemInfo>(JsonSerializerOptions)
            : null;
    }

    public void UpdateInfo(
        ProblemId id,
        string? input,
        string? part1Answer,
        bool? part1IsAccepted,
        string? part2Answer,
        bool? part2IsAccepted)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ProblemInfo newValue = new(
            id,
            SessionIdHash,
            input,
            part1Answer,
            part1IsAccepted is null ? null : now,
            part1IsAccepted,
            part2Answer,
            part2IsAccepted is null ? null : now,
            part2IsAccepted,
            now);

        AddOrUpdate(
            id.ToString(),
            _ => newValue,
            (_, existingValue) => existingValue.Patch(newValue));
    }

    public void SetInput(ProblemId id, string input)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ProblemInfo newValue = new(
            id,
            SessionIdHash,
            input,
            null,
            null,
            null,
            null,
            null,
            null,
            now);

        AddOrUpdate(
            id.ToString(),
            _ => newValue,
            (_, existingValue) => existingValue.Patch(newValue));
    }
}
