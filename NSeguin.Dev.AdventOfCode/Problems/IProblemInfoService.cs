namespace NSeguin.Dev.AdventOfCode;

public interface IProblemInfoService
{
    ValueTask<ProblemInfo> GetProblemInfoAsync(
        ProblemId id,
        CancellationToken cancellationToken = default);
}
