using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Timer = System.Timers.Timer;

namespace NSeguin.Dev.AdventOfCode;

internal sealed class JsonFileCacheManager : IHostedService, IDisposable
{
    private readonly Timer _flushTimer = new(TimeSpan.FromSeconds(15)) {AutoReset = false};
    private bool _isFlushing;
    private bool _isDisposed;

    private ValueTask? _shutdownTask;

    public JsonFileCacheManager(
        IJsonFileCacheRegistry registry,
        ILogger<JsonFileCacheManager> logger,
        IHostApplicationLifetime lifetime)
    {
        Logger = logger;
        Registry = registry;
        Registry.Added += (_, cache) =>
        {
            Logger.LogDebug("Registered cache {Cache}", cache.FileName);
            cache.LoadAsync().AsTask().Wait();
        };

        // Register the hosted service.
        lifetime.ApplicationStopping.Register(BeginShutdownAsync);
        lifetime.ApplicationStopped.Register(TryWaitForShutdownAsync);
        lifetime.ApplicationStarted.Register(BeginInitializationAsync);
    }

    private ILogger<JsonFileCacheManager> Logger { get; }

    private IJsonFileCacheRegistry Registry { get; }
    private IEnumerable<JsonFileCache> Caches => Registry.Caches;

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _flushTimer.Dispose();
        }

        _isDisposed = true;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _flushTimer.Elapsed += async (_, _) =>
        {
            Logger.LogDebug("Flushing cache");
            if (_isFlushing)
            {
                return;
            }

            _isFlushing = true;
            try
            {
                List<Exception> exceptions = new();
                foreach (JsonFileCache cache in Caches)
                {
                    try
                    {
                        await cache.SaveAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to flush cache");
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count > 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to flush cache");
            }
            finally
            {
                _isFlushing = false;
            }
        };

        _flushTimer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _flushTimer.Stop();
        return Task.CompletedTask;
    }

    private async void BeginInitializationAsync()
    {
        await Task.WhenAll(Caches.Select(cache => cache.LoadAsync().AsTask()))
            .ConfigureAwait(false);
    }

    private async void BeginShutdownAsync()
    {
        _shutdownTask = new ValueTask(
            Task.WhenAll(Caches.Select(cache => cache.SaveAsync().AsTask())));

        await _shutdownTask.Value.ConfigureAwait(false);
    }

    private void TryWaitForShutdownAsync()
    {
        if (_shutdownTask is null)
        {
            return;
        }

        if (_shutdownTask.Value.IsCompleted)
        {
            return;
        }

        TimeSpan timeout = TimeSpan.FromSeconds(2);
        Logger.LogDebug("Waiting for shutdown to complete (timeout: {Timeout})", timeout);
        Task.WaitAny(_shutdownTask.Value.AsTask(), Task.Delay(timeout));
    }
}
