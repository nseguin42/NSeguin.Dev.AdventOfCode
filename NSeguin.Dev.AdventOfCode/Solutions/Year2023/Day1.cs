using System.Buffers;
using System.Text.RegularExpressions;

namespace NSeguin.Dev.AdventOfCode.Solutions.Year2023;

public partial class Day1() : AdventOfCodeSolution(2023, 1)
{
    public override ValueTask<string> SolvePart1Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(Part1.Solve(input).ToString());
    }

    public override ValueTask<string> SolvePart2Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(Part2.GetTotal(input).ToString());
    }

    internal static class Part1
    {
        private static readonly SearchValues<char> Digits = SearchValues.Create("0123456789");

        public static int Solve(string input)
        {
            string[] lines = input.Trim().Split(Environment.NewLine);
            int total = lines.Sum(
                line =>
                {
                    ReadOnlySpan<char> chars = line.AsSpan();
                    int indexOfFirstNumber = chars.IndexOfAny(Digits);
                    int indexOfLastNumber = chars.LastIndexOfAny(Digits);
                    int firstNumber = int.Parse([chars[indexOfFirstNumber]]);
                    int lastNumber = int.Parse([chars[indexOfLastNumber]]);
                    return (10 * firstNumber) + lastNumber;
                });

            return total;
        }
    }

    internal static partial class Part2
    {
        private static readonly Dictionary<string, int> WordToDigit = new()
        {
            {"one", 1},
            {"two", 2},
            {"three", 3},
            {"four", 4},
            {"five", 5},
            {"six", 6},
            {"seven", 7},
            {"eight", 8},
            {"nine", 9},
            {"zero", 0}
        };

        [GeneratedRegex(@"((?<digit>\d)|(?<word>one|two|three|four|five|six|seven|eight|nine))")]
        public static partial Regex FirstNumberRegex();

        [GeneratedRegex(
            @"((?<digit>\d)|(?<word>one|two|three|four|five|six|seven|eight|nine)).*?$",
            RegexOptions.RightToLeft)]
        public static partial Regex LastNumberRegex();

        public static int GetTotal(string input)
        {
            string[] lines = input.Trim().Split(Environment.NewLine);
            return lines.Sum(GetLineValue);
        }

        public static int GetFirstNumber(string line)
        {
            Match match = FirstNumberRegex().Match(line);
            if (match.Groups["digit"].Success)
            {
                return int.Parse(match.Groups["digit"].Value);
            }

            if (match.Groups["word"].Success)
            {
                return WordToDigit[match.Groups["word"].Value];
            }

            throw new InvalidOperationException("No match was found.");
        }

        public static int GetLastNumber(string line)
        {
            Match match = LastNumberRegex().Match(line);
            if (match.Groups["digit"].Success)
            {
                return int.Parse(match.Groups["digit"].Value);
            }

            if (match.Groups["word"].Success)
            {
                return WordToDigit[match.Groups["word"].Value];
            }

            throw new InvalidOperationException("No match was found.");
        }

        private static int GetLineValue(string line)
        {
            int firstNumber = GetFirstNumber(line);
            int lastNumber = GetLastNumber(line);
            return (10 * firstNumber) + lastNumber;
        }
    }
}
