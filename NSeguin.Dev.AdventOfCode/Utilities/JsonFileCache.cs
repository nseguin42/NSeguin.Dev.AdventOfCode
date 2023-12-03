using System.Collections.Concurrent;
using System.Text.Json;

namespace NSeguin.Dev.AdventOfCode;

internal class JsonFileCache
{
    private bool _isLoaded;
    private bool _isTainted;

    public JsonFileCache(
        JsonFileCacheRegistry registry,
        string cacheFile,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        File = new FileInfo(cacheFile);
        JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();
        registry.Register(this);
    }

    public string FileName => File.FullName;
    protected ConcurrentDictionary<string, JsonElement> Values { get; } = new();

    protected JsonSerializerOptions JsonSerializerOptions { get; }

    private FileInfo File { get; }

    public T? GetValueOrDefault<T>(string key)
    {
        CheckLoaded();
        if (!Values.TryGetValue(key, out JsonElement value))
        {
            return default;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Null => default,
            JsonValueKind.Undefined => default,
            _ => JsonSerializer.Deserialize<T>(value.GetRawText(), JsonSerializerOptions)
        };
    }

    public T AddOrUpdate<T>(
        string key,
        Func<string, T> addValueFactory,
        Func<string, T, T> updateValueFactory)
    {
        CheckLoaded();
        Taint();
        T value = Values.AddOrUpdate(
                key,
                s => JsonSerializer.SerializeToElement(addValueFactory(s), JsonSerializerOptions),
                (s, oldValue) => JsonSerializer.SerializeToElement(
                    updateValueFactory(
                        s,
                        JsonSerializer.Deserialize<T>(
                            oldValue.GetRawText(),
                            JsonSerializerOptions)!),
                    JsonSerializerOptions))
            .Deserialize<T>(JsonSerializerOptions)!;

        return value;
    }

    public T? Get<T>(string key)
    {
        CheckLoaded();
        if (Values.TryGetValue(key, out JsonElement value))
        {
            return value.Deserialize<T>(JsonSerializerOptions);
        }

        throw new KeyNotFoundException();
    }

    public void Set<T>(string key, T value)
    {
        CheckLoaded();
        Values[key] = JsonSerializer.SerializeToElement(value, JsonSerializerOptions);
        Taint();
    }

    public async ValueTask SaveAsync(CancellationToken cancellationToken = default)
    {
        CheckLoaded();
        if (!_isTainted)
        {
            return;
        }

        if (!File.Exists)
        {
            Directory.CreateDirectory(File.DirectoryName!);
        }

        await using FileStream fileStream = File.OpenWrite();
        await JsonSerializer.SerializeAsync(
            fileStream,
            Values.ToDictionary(pair => pair.Key, pair => pair.Value),
            JsonSerializerOptions,
            cancellationToken);
    }

    public async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        Initialize();
        if (File.Exists)
        {
            await using FileStream fileStream = File.OpenRead();
            Dictionary<string, JsonElement>? cache
                = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(
                    fileStream,
                    JsonSerializerOptions,
                    cancellationToken);

            if (cache is null)
            {
                throw new InvalidOperationException("Failed to deserialize cache");
            }

            foreach ((string key, JsonElement value) in cache)
            {
                Values.TryAdd(key, value);
            }
        }

        _isLoaded = true;
    }

    protected async ValueTask EnsureLoadedAsync()
    {
        if (_isLoaded)
        {
            return;
        }

        await LoadAsync().ConfigureAwait(false);
    }

    private void CheckLoaded()
    {
        if (!_isLoaded)
        {
            throw new InvalidOperationException("Cache is not loaded");
        }
    }

    private void Initialize()
    {
        Values.Clear();
        _isTainted = false;
    }

    private void Taint()
    {
        _isTainted = true;
    }
}
