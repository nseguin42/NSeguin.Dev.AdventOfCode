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
        throw new NotImplementedException();
    }

    internal static class Day1
    {
        public static int GetTotal(string input)
        {
            var schematic = new Schematic(input);
            var initialCharsOfNumbers = schematic.Chars.SelectMany(x => x)
                .Where(schematic.IsStartOfNumber)
                .ToList();

            var numberSpansWhereAnyIsAdjacentToSymbol = initialCharsOfNumbers.Select(
                    c =>
                    {
                        int startOfNumber = c.X;
                        (int x, int y) = (c.X, c.Y);
                        bool isAdjacentToSymbol = false;
                        do
                        {
                            isAdjacentToSymbol |= schematic.IsAdjacentToSymbolAt(x, y);
                            x++;
                        }
                        while (x < schematic.Chars[y].Length && schematic.IsNumberAt(x, y));

                        if (isAdjacentToSymbol)
                        {
                            return schematic.Chars[y][startOfNumber..x]
                                .Select(x => x.Char)
                                .ToArray();
                        }

                        return null;
                    })
                .Where(x => x is not null)
                .ToList();

            var numbersAdjacentToSymbol = numberSpansWhereAnyIsAdjacentToSymbol
                .Select(x => int.Parse(x))
                .ToList();

            var total = numbersAdjacentToSymbol.Sum();
            return total;
        }
    }

    public partial class Schematic

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

            PositionsAdjacentToSymbols = Chars
                .SelectMany((row, y) => row.Select((c, x) => (c, x, y)))
                .Where(t => t.c.IsSymbol)
                .SelectMany(
                    t => new[]
                    {
                        (t.x - 1, t.y - 1), (t.x, t.y - 1), (t.x + 1, t.y - 1), (t.x - 1, t.y),
                        (t.x + 1, t.y), (t.x - 1, t.y + 1), (t.x, t.y + 1), (t.x + 1, t.y + 1),
                    })
                .ToHashSet();
        }

        public CharInfo[][] Chars { get; }

        public HashSet<(int X, int Y)> PositionsAdjacentToSymbols { get; }

        public CharInfo GetCharAt(int x, int y)
        {
            return Chars[y][x];
        }

        public bool IsStartOfNumber(CharInfo c)
        {
            return c.IsNumber && (c.X == 0 || !GetCharAt(c.X - 1, c.Y).IsNumber);
        }

        public bool IsSymbolAt(int x, int y)
        {
            return GetCharAt(x, y).IsSymbol;
        }

        public bool IsNumberAt(int x, int y)
        {
            return GetCharAt(x, y).IsNumber;
        }

        public bool IsAdjacentToSymbolAt(int x, int y)
        {
            return PositionsAdjacentToSymbols.Contains((x, y));
        }

        public class CharInfo(char c, int x, int y)
        {
            public char Char { get; } = c;
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
            public bool IsNumber => char.IsDigit(Char);

            public bool IsSymbol => !IsNumber && Char != '.';
        }
    }
}
