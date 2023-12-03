namespace NSeguin.Dev.AdventOfCode;

internal interface IJsonFileCacheRegistry
{
    event EventHandler<JsonFileCache>? Added;
    IEnumerable<JsonFileCache> Caches { get; }
    void Register<T>(T cache) where T : JsonFileCache;
}
