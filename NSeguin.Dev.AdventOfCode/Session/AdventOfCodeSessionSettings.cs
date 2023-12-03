using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NSeguin.Dev.AdventOfCode;

public class AdventOfCodeSessionSettings
{
    [Required]
    [DisallowNull]
    public string? SessionId { get; set; }
}
