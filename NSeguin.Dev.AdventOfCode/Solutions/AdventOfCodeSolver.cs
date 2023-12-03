using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NSeguin.Dev.AdventOfCode.Solutions;

public sealed class AdventOfCodeSolver(
    ILogger<AdventOfCodeSolver> logger,
    IEnumerable<AdventOfCodeSolution> solutions,
    IAdventOfCodeClient client,
    IProblemInfoService problemInfoService,
    IOptions<AdventOfCodeSettings> settings)
{
    private ILogger<AdventOfCodeSolver> Logger { get; } = logger;

    private IAdventOfCodeClient Client { get; } = client;

    private Dictionary<ProblemId, AdventOfCodeSolution> Solutions { get; }
        = solutions.ToDictionary(s => s.Id);

    private IProblemInfoService ProblemInfoService { get; } = problemInfoService;

    private AdventOfCodeSettings Settings { get; } = settings.Value;

    public async ValueTask SolveAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (AdventOfCodeSolution solution in Solutions.Values)
        {
            await SolveAsync(solution.Id, cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask SolveAsync(ProblemId id, CancellationToken cancellationToken = default)
    {
        using IDisposable? scope = Logger.BeginScope(id);
        if (!Solutions.TryGetValue(id, out AdventOfCodeSolution? solution))
        {
            throw new InvalidOperationException("No solution found for problem");
        }

        string input = await GetInputAsync(id).ConfigureAwait(false);
        Logger.LogDebug("Problem input: {#ProblemInput}", input);
        int[] partsToSolve = {1, 2};
        foreach (int part in partsToSolve)
        {
            await SolvePartAsync(solution, part, input, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task SolvePartAsync(
        AdventOfCodeSolution solution,
        int part,
        string input,
        CancellationToken cancellationToken = default)
    {
        using IDisposable? scope
            = Logger.BeginScope(new {solution.Year, solution.Day, Part = part});

        ProblemId id = solution.Id;
        string output = part switch
        {
            1 => await solution.SolvePart1Async(input, cancellationToken).ConfigureAwait(false),
            2 => await solution.SolvePart2Async(input, cancellationToken).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };

        Logger.LogDebug("Solution: {#Solution}", output);
        if (Settings.SubmitAnswers)
        {
            await Client.SubmitProblemOutputAsync(id.Year, id.Day, part, output, cancellationToken)
                .ConfigureAwait(false);

            Logger.LogInformation("Solution submitted");
        }
        else
        {
            Logger.LogDebug("Skipping solution submission");
        }
    }

    private async ValueTask<string> GetInputAsync(ProblemId id)
    {
        ProblemInfo problemInfo = await ProblemInfoService.GetProblemInfoAsync(id)
            .ConfigureAwait(false);

        return problemInfo.Input ?? throw new InvalidOperationException("No input found");
    }
}
