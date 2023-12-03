namespace NSeguin.Dev.AdventOfCode.Solutions;

public abstract class AdventOfCodeSolution(ProblemId id)
{
    protected AdventOfCodeSolution(int year, int day) : this(new ProblemId(year, day))
    {
    }

    public ProblemId Id { get; } = id;

    public int Year => Id.Year;

    public int Day => Id.Day;

    public abstract ValueTask<string> SolvePart1Async(
        string input,
        CancellationToken cancellationToken = default);

    public abstract ValueTask<string> SolvePart2Async(
        string input,
        CancellationToken cancellationToken = default);
}
