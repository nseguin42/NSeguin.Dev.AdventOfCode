using System.ComponentModel.DataAnnotations;

namespace NSeguin.Dev.AdventOfCode;

public readonly record struct ProblemId(
    [property: Range(2023, int.MaxValue)]
    int Year,
    [property: Range(1, 25)]
    int Day)
{
    public static implicit operator ProblemId((int Year, int Day) id)
    {
        return new ProblemId(id.Year, id.Day);
    }

    public override string ToString()
    {
        return $"Year {Year} Day {Day}";
    }
}
