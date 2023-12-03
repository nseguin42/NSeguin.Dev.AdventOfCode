using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NSeguin.Dev.AdventOfCode;

internal sealed class ProblemInfoService : IProblemInfoService
{
    private readonly SessionAccessor _sessionAccessor;
    private readonly Lazy<ValueTask> _initializationLazy;
    private readonly Lazy<ValueTask> _shutdownLazy;
    private bool _isFlushing;

    public ProblemInfoService(
        ILogger<ProblemInfoService> logger,
        IOptions<AdventOfCodeSettings> settings,
        SessionAccessor sessionAccessor,
        IAdventOfCodeClient client,
        IJsonFileCacheRegistry registry)
    {
        _sessionAccessor = sessionAccessor;
        Logger = logger;
        Cache = new ProblemInfoCache(registry, Session.Sha256Hash, settings.Value.CacheFileName);
        Client = client;
        _initializationLazy = new Lazy<ValueTask>(InitializeAsync);
        _shutdownLazy = new Lazy<ValueTask>(ShutdownAsync);
    }

    public ValueTask Initialization => _initializationLazy.Value;

    public ValueTask Shutdown => _shutdownLazy.Value;

    public bool IsShutdown { get; private set; }

    public bool IsInitialized { get; private set; }

    private IAdventOfCodeClient Client { get; }

    private ILogger<ProblemInfoService> Logger { get; }

    private bool CanFlush => IsInitialized && !IsShutdown && !_isFlushing;

    private Session Session => _sessionAccessor.Session;
    private string CacheSessionIdHash => Cache.SessionIdHash;

    private ProblemInfoCache Cache { get; }

    public async Task SaveCacheAsync()
    {
        if (!CanFlush)
        {
            return;
        }

        _isFlushing = true;
        try
        {
            await Cache.SaveAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to flush cache");
        }
        finally
        {
            _isFlushing = false;
        }
    }

    public async Task LoadCacheAsync()
    {
        Logger.LogDebug("Loading cache");
        try
        {
            await Cache.LoadAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize cache");
        }
    }

    public void AddProblemInput(ProblemId id, string input)
    {
        CheckCacheExpiry();
        Cache.SetInput(id, input);
    }

    public async ValueTask<ProblemInfo> GetProblemInfoAsync(
        ProblemId id,
        CancellationToken cancellationToken = default)
    {
        ProblemInfo? problemInfo = await GetProblemInfoFromCacheAsync(id, cancellationToken)
            .ConfigureAwait(false);

        if (problemInfo is not null)
        {
            return problemInfo;
        }

        string? input = await Client.TryGetProblemInfoAsync(id.Year, id.Day, cancellationToken)
            .ConfigureAwait(false);

        if (input is null)
        {
            throw new InvalidOperationException("Failed to get problem info");
        }

        problemInfo = new ProblemInfo(
            id,
            Session.Sha256Hash,
            input,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        Cache.Set(id.ToString(), problemInfo);
        return problemInfo;
    }

    private async ValueTask<ProblemInfo?> GetProblemInfoFromCacheAsync(
        ProblemId id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Initialization.ConfigureAwait(false);
        CheckCacheExpiry();
        return await Cache.GetProblemInfoAsync(id);
    }

    private async ValueTask InitializeAsync()
    {
        if (!IsInitialized)
        {
            await LoadCacheAsync().ConfigureAwait(false);
        }

        IsInitialized = true;
    }

    private async ValueTask ShutdownAsync()
    {
        if (!IsShutdown)
        {
            await SaveCacheAsync().ConfigureAwait(false);
        }

        IsShutdown = true;
    }

    private void CheckCacheExpiry()
    {
        if (Session is null)
        {
            throw new InvalidOperationException("Session is not initialized");
        }

        if (CacheSessionIdHash != Session.Sha256Hash)
        {
            throw new InvalidOperationException(
                "Cache session id hash does not match the current session");
        }
    }
}
