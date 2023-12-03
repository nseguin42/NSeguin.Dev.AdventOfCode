using System.Net;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using NSeguin.Dev.AdventOfCode.Solutions;

using Polly;

namespace NSeguin.Dev.AdventOfCode;

public static class ServiceRegistration
{
    public static IServiceCollection AddAdventOfCode(
        this IServiceCollection services,
        Assembly assemblyContainingSolutions)
    {
        services.AddAdventOfCode();
        services.AddAdventOfCodeSolutionsFromAssembly(assemblyContainingSolutions);
        return services;
    }

    public static IServiceCollection AddAdventOfCode(this IServiceCollection services)
    {
        services.AddSingleton<IProblemInfoService, ProblemInfoService>();
        services.AddSingleton<JsonFileCacheRegistry, JsonFileCacheRegistry>();
        services.AddHostedService<JsonFileCacheManager>();
        services.AddSingleton<SessionAccessor>();
        services.AddSingleton<AdventOfCodeSolver>();
        services.AddHttpClient<IAdventOfCodeClient, AdventOfCodeClient>()
            .AddTransientHttpErrorPolicy(
                builder => builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30)))
            .AddPolicyHandler(
                Policy.HandleResult<HttpResponseMessage>(
                        response => response.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt + 2))))
            .AddPolicyHandler(
                Policy.RateLimitAsync<HttpResponseMessage>(1, TimeSpan.FromSeconds(5), 10))
            .ConfigurePrimaryHttpMessageHandler(
                services =>
                {
                    AdventOfCodeSettings options
                        = services.GetRequiredService<IOptions<AdventOfCodeSettings>>().Value;

                    Session session = services.GetRequiredService<SessionAccessor>().Session;
                    HttpClientHandler handler = new();
                    handler.CookieContainer = new CookieContainer();
                    handler.CookieContainer.Add(
                        new Cookie(
                            "session",
                            session.SessionId
                            ?? throw new InvalidOperationException("Session ID is not set"),
                            "/",
                            new Uri(options.BaseUrl!, UriKind.Absolute).Host));

                    handler.AutomaticDecompression = DecompressionMethods.GZip
                                                     | DecompressionMethods.Deflate
                                                     | DecompressionMethods.Brotli;

                    handler.UseCookies = true;
                    handler.UseProxy = false;
                    return handler;
                });

        return services;
    }

    public static IServiceCollection AddAdventOfCodeSolutionsFromAssembly(
        this IServiceCollection services,
        Assembly assemblyContainingSolutions)
    {
        IEnumerable<Type> types = assemblyContainingSolutions.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(AdventOfCodeSolution)) && !type.IsAbstract);

        foreach (Type type in types)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton(typeof(AdventOfCodeSolution), type));
        }

        return services;
    }
}
