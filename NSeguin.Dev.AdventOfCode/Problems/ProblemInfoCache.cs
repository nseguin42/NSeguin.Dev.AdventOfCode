using System.Text.Json;

namespace NSeguin.Dev.AdventOfCode;

internal class ProblemInfoCache : JsonFileCache
{
    public ProblemInfoCache(JsonFileCacheRegistry registry, string sessionIdHash, string fileName) :
        base(registry, fileName, ProblemInfoCacheSerializerContext.Default.Options)
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

    public void UpdateProblemInfo(ProblemId id, int part, string answer, bool isAccepted)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ProblemInfo newValue = new(
            id,
            SessionIdHash,
            null,
            part == 1 ? answer : null,
            part == 1 ? now : null,
            part == 1 ? isAccepted : null,
            part == 2 ? answer : null,
            part == 2 ? now : null,
            part == 2 ? isAccepted : null,
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
