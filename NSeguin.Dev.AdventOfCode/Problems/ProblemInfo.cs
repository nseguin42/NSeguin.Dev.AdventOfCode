namespace NSeguin.Dev.AdventOfCode;

public record ProblemInfo(
    ProblemId Id,
    string SessionIdSha256Hash,
    string? Input,
    string? Part1Answer,
    DateTimeOffset? Part1SubmittedAt,
    bool? Part1IsAccepted,
    string? Part2Answer,
    DateTimeOffset? Part2SubmittedAt,
    bool? Part2IsAccepted,
    DateTimeOffset? LastUpdatedAt);
