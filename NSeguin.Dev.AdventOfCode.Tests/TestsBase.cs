using Xunit.Abstractions;

namespace NSeguin.Dev.AdventOfCode.Tests;

public abstract class TestsBase
{
    protected TestsBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected ITestOutputHelper Output { get; }
}
