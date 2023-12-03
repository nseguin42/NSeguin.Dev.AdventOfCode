using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSeguin.Dev.AdventOfCode.Solutions;

namespace NSeguin.Dev.AdventOfCode;

public sealed class AdventOfCodeRunner
{
    public AdventOfCodeRunner(
        ILogger<AdventOfCodeRunner> logger,
        IOptions<AdventOfCodeSettings> options,
        AdventOfCodeSolver solver)
    {
        Logger = logger;
        Options = options.Value;
        Solver = solver;
    }

    private ILogger<AdventOfCodeRunner> Logger { get; }

    private AdventOfCodeSettings Options { get; }

    private AdventOfCodeSolver Solver { get; }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Starting Advent of Code runner");
        if (Options.ProblemsToSolve is null)
        {
            await Solver.SolveAllAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            foreach (ProblemId id in Options.ProblemsToSolve)
            {
                await Solver.SolveAsync(id, cancellationToken).ConfigureAwait(false);
            }
        }

        Logger.LogInformation("Advent of Code runner finished");
    }
}
