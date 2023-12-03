using NSeguin.Dev.AdventOfCode.Solutions.Year2023;

using Xunit.Abstractions;

namespace NSeguin.Dev.AdventOfCode.Tests.Solutions;

public class Day1Tests : TestsBase
{
    private const string Example = """
                                   two1nine
                                   eightwothree
                                   abcone2threexyz
                                   xtwone3four
                                   4nineeightseven2
                                   zoneight234
                                   7pqrstsixteen
                                   """;

    private const int ExampleTotal = 281;

    public Day1Tests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData(Example, ExampleTotal)]
    public void Part2_GetTotal(string input, int expectedTotal)
    {
        int total = Day1.Part2.GetTotal(input);

        Output.WriteLine(input);
        Output.WriteLine($"Total: {total}");

        Assert.Equal(expectedTotal, total);
    }

    [Theory]
    [InlineData("7pdjjjfcmq26four", 7, 4)]
    [InlineData("two1nine", 2, 9)]
    [InlineData("eightwothree", 8, 3)]
    [InlineData("abcone2threexyz", 1, 3)]
    [InlineData("xtwone3four", 2, 4)]
    [InlineData("4nineeightseven2", 4, 2)]
    [InlineData("zoneight234", 1, 4)]
    [InlineData("7pqrstsixteen", 7, 6)]
    [InlineData("eightwonine", 8, 9)]
    public void Part2_GetNumbers(string input, int expectedFirstNumber, int expectedLastNumber)
    {
        int firstNumber = Day1.Part2.GetFirstNumber(input);
        int lastNumber = Day1.Part2.GetLastNumber(input);

        Output.WriteLine(input);
        Output.WriteLine($"First Number: {firstNumber}");
        Output.WriteLine($"Last Number: {lastNumber}");

        firstNumber.Should().Be(expectedFirstNumber);
        lastNumber.Should().Be(expectedLastNumber);
    }
}
