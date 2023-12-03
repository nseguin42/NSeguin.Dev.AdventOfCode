namespace NSeguin.Dev.AdventOfCode;

public interface IProblemInfoService
{
    ValueTask<ProblemInfo> GetProblemInfoAsync(
        ProblemId id,
        CancellationToken cancellationToken = default);

    void UpdateProblemInfo(ProblemId id, int part, string answer, bool isAccepted);
}
