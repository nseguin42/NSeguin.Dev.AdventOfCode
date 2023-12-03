using System.Diagnostics.CodeAnalysis;

namespace NSeguin.Dev.AdventOfCode.Solutions.Year2023;

public partial class Day3() : AdventOfCodeSolution(2023, 3)
{
    public override ValueTask<string> SolvePart1Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(Day1.GetTotal(input).ToString());
    }

    public override ValueTask<string> SolvePart2Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(Day2.GetTotal(input).ToString());
    }

    public class AdjacentToComparer : IEqualityComparer<Schematic.CharInfo>
    {
        public static AdjacentToComparer Default { get; } = new();

        public bool Equals(Schematic.CharInfo x, Schematic.CharInfo y)
        {
            return Math.Abs(x.X - y.X) <= 1 && Math.Abs(x.Y - y.Y) <= 1;
        }

        public int GetHashCode(Schematic.CharInfo obj)
        {
            return 0;
        }
    }

    public class Schematic
    {
        public Schematic(string input)
        {
            Chars = input.Split(Environment.NewLine)
                .Select(
                    (l, y) => l.Trim()
                        .ToCharArray()
                        .Select((c, x) => new CharInfo(c, x, y))
                        .ToArray())
                .ToArray();

            PositionsAdjacentToSymbols = Chars.SelectMany(x => x)
                .Where(c => c.IsSymbol)
                .ToHashSet(AdjacentToComparer.Default);
        }

        public CharInfo[][] Chars { get; }

        public HashSet<CharInfo> PositionsAdjacentToSymbols { get; }

        public CharInfo GetCharAt(int x, int y)
        {
            return Chars[y][x];
        }

        public bool IsStartOfNumber(CharInfo c)
        {
            return c.IsNumber && (c.X == 0 || !GetCharAt(c.X - 1, c.Y).IsNumber);
        }

        public bool IsAdjacentToSymbol(CharInfo c, out CharInfo adjacentToCharInfo)
        {
            return PositionsAdjacentToSymbols.TryGetValue(c, out adjacentToCharInfo);
        }

        public readonly struct CharInfo(char c, int x, int y)
        {
            public char Char { get; } = c;
            public int X { get; } = x;
            public int Y { get; } = y;
            public bool IsNumber => char.IsDigit(Char);

            public bool IsSymbol => !IsNumber && Char != '.';
            public bool IsGear => Char == '*';
        }
    }

    internal static class Day1
    {
        public static int GetTotal(string input)
        {
            var schematic = new Schematic(input);
            var initialCharsOfNumbers = schematic.Chars.SelectMany(x => x)
                .Where(schematic.IsStartOfNumber)
                .ToList();

            var numbersAdjacentToSymbol = initialCharsOfNumbers.Select(
                    c =>
                    {
                        int startOfNumber = c.X;
                        int row = c.Y;
                        int rowLength = schematic.Chars[row].Length;
                        int currentX = c.X;
                        List<Schematic.CharInfo> adjacentTo = [];
                        do
                        {
                            var charInfo = schematic.GetCharAt(currentX, row);
                            if (schematic.IsAdjacentToSymbol(charInfo, out var adjacentToCharInfo))
                            {
                                adjacentTo.Add(adjacentToCharInfo);
                            }

                            currentX++;
                        }
                        while (currentX < rowLength - 1
                               && schematic.GetCharAt(currentX, row).IsNumber);

                        if (adjacentTo.Count > 0)
                        {
                            var numberSpan = schematic.Chars[row][startOfNumber..currentX]
                                .Select(x => x.Char)
                                .ToArray();

                            var number = int.Parse(numberSpan);
                            return (Number: number, AdjacentTo: adjacentTo);
                        }

                        return default;
                    })
                .Where(x => x != default)
                .ToList();

            var total = numbersAdjacentToSymbol.Sum(x => x.Number);
            return total;
        }
    }

    internal static class Day2
    {
        public static int GetTotal(string input)
        {
            var schematic = new Schematic(input);
            var initialCharsOfNumbers = schematic.Chars.SelectMany(x => x)
                .Where(schematic.IsStartOfNumber)
                .ToList();

            var numbersAdjacentToSymbol = initialCharsOfNumbers.Select(
                    c =>
                    {
                        int startOfNumber = c.X;
                        int row = c.Y;
                        int rowLength = schematic.Chars[row].Length;
                        int currentX = c.X;
                        List<Schematic.CharInfo> adjacentTo = [];
                        do
                        {
                            var charInfo = schematic.GetCharAt(currentX, row);
                            if (schematic.IsAdjacentToSymbol(charInfo, out var adjacentToCharInfo))
                            {
                                adjacentTo.Add(adjacentToCharInfo);
                            }

                            currentX++;
                        }
                        while (currentX < rowLength - 1
                               && schematic.GetCharAt(currentX, row).IsNumber);

                        var adjacentToGears = adjacentTo.Where(c => c.IsGear)
                            .DistinctBy(c => (c.X, c.Y))
                            .ToList();

                        if (adjacentToGears.Count > 0)
                        {
                            var numberSpan = schematic.Chars[row][startOfNumber..currentX]
                                .Select(x => x.Char)
                                .ToArray();

                            var number = int.Parse(numberSpan);
                            return (Number: number, AdjacentToGears: adjacentToGears);
                        }

                        return default;
                    })
                .Where(x => x != default)
                .ToList();

            var numbersGroupedByGear = numbersAdjacentToSymbol
                .SelectMany(x => x.AdjacentToGears.Select(g => (x.Number, Gear: g)))
                .GroupBy(x => x.Gear)
                .Where(x => x.Count() == 2)
                .Select(x => (First: x.First(), Second: x.Last()))
                .ToList();

            var total = numbersGroupedByGear.Sum(x => x.First.Number * x.Second.Number);
            return total;
        }
    }
}
