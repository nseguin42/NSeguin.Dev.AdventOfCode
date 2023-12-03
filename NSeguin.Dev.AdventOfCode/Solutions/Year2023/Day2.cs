namespace NSeguin.Dev.AdventOfCode.Solutions.Year2023;

public class Day2() : AdventOfCodeSolution(2023, 2)
{
    public override ValueTask<string> SolvePart1Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        int total = Part1.GetTotal(input);
        return new ValueTask<string>(total.ToString());
    }

    public override ValueTask<string> SolvePart2Async(
        string input,
        CancellationToken cancellationToken = default)
    {
        int total = Part2.GetTotal(input);
        return new ValueTask<string>(total.ToString());
    }

    public record Game(int Id, List<GameTurn> Turns)
    {
        public static Game FromString(string input)
        {
            if (input.Contains(Environment.NewLine))
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            string[] parts = input.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            string id = parts[0].Replace("Game ", string.Empty);
            string[] gameTurnsStr = parts[1].Split(';');
            List<GameTurn> gameTurns = gameTurnsStr.Select(GameTurn.FromString).ToList();
            return new Game(int.Parse(id), gameTurns);
        }
    }

    public record GameTurn(int RedCubes, int GreenCubes, int BlueCubes)
    {
        public static GameTurn FromString(string input)
        {
            List<string> gameTurnParts = input.Split(',').Select(x => x.Trim()).ToList();
            int red = 0;
            int green = 0;
            int blue = 0;
            foreach (string gameTurnPart in gameTurnParts)
            {
                string[] gameTurnPartParts = gameTurnPart.Split(' ');
                string count = gameTurnPartParts[0];
                string color = gameTurnPartParts[1];
                switch (color)
                {
                    case "red":
                        red = int.Parse(count);
                        break;
                    case "green":
                        green = int.Parse(count);
                        break;
                    case "blue":
                        blue = int.Parse(count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color));
                }
            }

            return new GameTurn(red, green, blue);
        }
    }

    public record Bag(int RedCubes, int GreenCubes, int BlueCubes)
    {
        public int Power => RedCubes * GreenCubes * BlueCubes;
    }

    internal static class Part1
    {
        public const int MaxRedCubes = 12;
        public const int MaxGreenCubes = 13;
        public const int MaxBlueCubes = 14;

        public static int GetTotal(string input)
        {
            List<Game> games = input.Split(Environment.NewLine)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(Game.FromString)
                .Where(IsPossible)
                .ToList();

            return games.Sum(g => g.Id);
        }

        private static bool IsPossible(Game game)
        {
            return game.Turns.All(IsPossible);
        }

        private static bool IsPossible(GameTurn gameTurn)
        {
            return gameTurn is
            {
                RedCubes: <= MaxRedCubes, GreenCubes: <= MaxGreenCubes, BlueCubes: <= MaxBlueCubes
            };
        }
    }

    internal static class Part2
    {
        public static int GetTotal(string input)
        {
            List<Game> games = input.Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(Game.FromString)
                .ToList();

            List<Bag> bags = games.Select(GetSmallestBagForGame).ToList();
            return bags.Sum(x => x.Power);
        }

        private static Bag GetSmallestBagForGame(Game game)
        {
            int redCubes = game.Turns.Max(x => x.RedCubes);
            int greenCubes = game.Turns.Max(x => x.GreenCubes);
            int blueCubes = game.Turns.Max(x => x.BlueCubes);
            return new Bag(redCubes, greenCubes, blueCubes);
        }
    }
}
