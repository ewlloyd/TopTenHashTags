using Xunit.Abstractions;

namespace TopTenHashTags.Tests;

public class LeaderboardTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LeaderboardTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("a", 2, "z", 1)]
    [InlineData("z", 2, "a", 1)]
    [InlineData("a", 2, "z", 2)]
    public void EntryComparisonsWork(string entryOneKey, int entryOneCount, string entryTwoKey, int entryTwoCount)
    {
        var one = new Leaderboard<string>.Entry(entryOneKey, entryOneCount);
        var two = new Leaderboard<string>.Entry(entryTwoKey, entryTwoCount);
        Assert.True(one < two);
    }

    [Fact]
    public void LeaderboardConsumesData()
    {
        var sut = new Leaderboard<string>(3);
        sut.Tally("a");
        sut.Tally("c");
        sut.Tally("c");
        sut.Tally("b");
        sut.Tally("b");
        sut.Tally("b");
        sut.Tally("z");

        foreach (var result in sut.Results)
            _testOutputHelper.WriteLine(result.ToString());

        Assert.Equal(3, sut.Results.Count);
        Assert.Equal(new Leaderboard<string>.Result("b", 3), sut.Results[0]);
        Assert.Equal(new Leaderboard<string>.Result("c", 2), sut.Results[1]);
        Assert.Equal(new Leaderboard<string>.Result("a", 1), sut.Results[2]);
    }
}