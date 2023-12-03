namespace NSeguin.Dev.AdventOfCode;

internal class JsonFileCacheRegistry
{
    private readonly List<JsonFileCache> _caches = [];
    public event EventHandler<JsonFileCache>? Added;
    public IEnumerable<JsonFileCache> Caches => _caches.AsReadOnly();

    public void Register<T>(T cache) where T : JsonFileCache
    {
        _caches.Add(cache);
        Added?.Invoke(this, cache);
    }
}
