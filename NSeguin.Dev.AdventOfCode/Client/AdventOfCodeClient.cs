using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

using Microsoft.Extensions.Options;

namespace NSeguin.Dev.AdventOfCode;

public sealed class AdventOfCodeClient : IAdventOfCodeClient
{
    public AdventOfCodeClient(HttpClient httpClient, IOptions<AdventOfCodeSettings> settings)
    {
        HttpClient = httpClient;
        Settings = settings.Value;
        ConfigureHttpClient(httpClient, Settings);
    }

    private HttpClient HttpClient { get; }

    private AdventOfCodeSettings Settings { get; }

    public Task<string> TryGetProblemInfoAsync(
        int year,
        int day,
        CancellationToken cancellationToken = default)
    {
        return HttpClient.GetStringAsync($"/{year}/day/{day}/input", cancellationToken);
    }

    public async Task<bool> SubmitProblemOutputAsync(
        int year,
        int day,
        int part,
        string output,
        CancellationToken cancellationToken = default)
    {
        if (!Settings.SubmitAnswers)
        {
            throw new InvalidOperationException("Submitting answers is disabled");
        }

        UriBuilder uriBuilder = new($"{year}/day/{day}/answer");
        if (part == 2)
        {
            uriBuilder.Query = "level=2";
        }

        StringContent content = new(output, Encoding.UTF8, MediaTypeNames.Text.Plain);
        HttpResponseMessage response = await HttpClient
            .PostAsync(uriBuilder.Uri, content, cancellationToken)
            .ConfigureAwait(false);

        return response.IsSuccessStatusCode;
    }

    private static void ConfigureHttpClient(HttpClient httpClient, AdventOfCodeSettings settings)
    {
        ArgumentException.ThrowIfNullOrEmpty(settings.BaseUrl);
        httpClient.BaseAddress = new Uri(settings.BaseUrl, UriKind.Absolute);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Plain));

        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("NSeguin.Dev.AdventOfCode", "1.0.0"));

        httpClient.Timeout = TimeSpan.FromSeconds(settings.TimeoutInSeconds);
    }
}
