using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NSeguin.Dev.AdventOfCode;
using NSeguin.Dev.AdventOfCode.Solutions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(
    new HostApplicationBuilderSettings
    {
        EnvironmentName = Environments.Development, ApplicationName = "NSeguin.Dev.AdventOfCode"
    });

builder.Configuration.AddJsonFile("session.json", true);
builder.Logging.AddSimpleConsole();
builder.Services.AddAdventOfCode(typeof(AdventOfCodeSolver).Assembly);
builder.Services.AddOptionsWithValidateOnStart<AdventOfCodeSettings>()
    .BindConfiguration("AdventOfCode")
    .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<AdventOfCodeSessionSettings>()
    .BindConfiguration("AdventOfCode:Session")
    .ValidateDataAnnotations();

builder.Services.AddSingleton<AdventOfCodeRunner>();
builder.Services.AddMemoryCache();
using IHost host = builder.Build();
ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
await host.StartAsync();
try
{
    AdventOfCodeRunner runner = host.Services.GetRequiredService<AdventOfCodeRunner>();
    await runner.RunAsync(
        host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);
}
catch (Exception ex) when (ex is not OperationCanceledException or TaskCanceledException)
{
    logger.LogCritical(ex, "Stopping program due to unhandled exception");
    Environment.ExitCode = 1;
}

await host.StopAsync();
