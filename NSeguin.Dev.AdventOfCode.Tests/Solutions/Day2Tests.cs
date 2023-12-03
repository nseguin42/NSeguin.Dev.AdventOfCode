using NSeguin.Dev.AdventOfCode.Solutions.Year2023;

using Xunit.Abstractions;

namespace NSeguin.Dev.AdventOfCode.Tests.Solutions;

public class Day2Tests : TestsBase
{
    private const string Example = """
                                   Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                                   Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
                                   Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
                                   Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
                                   Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
                                   """;

    private const int Example_Part1_Total = 8;
    private const int Example_Part2_Total = 2286;

    public Day2Tests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(Example, Example_Part1_Total)]
    public void TestSolvePart1(string input, int expected)
    {
        int total = Day2.Part1.GetTotal(input);
        total.Should().Be(expected);
    }

    [Theory]
    [InlineData(Example, Example_Part2_Total)]
    public void TestSolvePart2(string input, int expected)
    {
        int total = Day2.Part2.GetTotal(input);
        total.Should().Be(expected);
    }

    [Fact]
    public void TestParseLine()
    {
        string line = "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green";
        Day2.Game game = Day2.Game.FromString(line);
        game.Id.Should().Be(1);
        game.Turns.Should().HaveCount(3);
        game.Turns[0].RedCubes.Should().Be(4);
        game.Turns[0].BlueCubes.Should().Be(3);
        game.Turns[0].GreenCubes.Should().Be(0);
        game.Turns[1].RedCubes.Should().Be(1);
        game.Turns[1].GreenCubes.Should().Be(2);
        game.Turns[1].BlueCubes.Should().Be(6);
        game.Turns[2].RedCubes.Should().Be(0);
        game.Turns[2].GreenCubes.Should().Be(2);
        game.Turns[2].BlueCubes.Should().Be(0);
    }
}
