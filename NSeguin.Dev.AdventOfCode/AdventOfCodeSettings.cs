using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NSeguin.Dev.AdventOfCode;

public class AdventOfCodeSettings
{
    [Required]
    [DisallowNull]
    public string? BaseUrl { get; set; }

    public bool SubmitAnswers { get; set; } = true;

    public int TimeoutInSeconds { get; set; } = 30;

    [Required]
    [DisallowNull]
    public List<ProblemId>? ProblemsToSolve { get; set; }

    [Required]
    [DisallowNull]
    public string CacheFileName { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NSeguin.Dev",
        "AdventOfCode",
        "cache.json");
}
