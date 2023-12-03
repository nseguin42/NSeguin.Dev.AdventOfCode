namespace NSeguin.Dev.AdventOfCode;

public interface IAdventOfCodeClient
{
    public Task<string> TryGetProblemInfoAsync(
        int year,
        int day,
        CancellationToken cancellationToken = default);

    public Task<bool> SubmitProblemOutputAsync(
        int year,
        int day,
        int part,
        string output,
        CancellationToken cancellationToken = default);
}
