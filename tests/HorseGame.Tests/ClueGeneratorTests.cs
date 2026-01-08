using Xunit;
using HorseGame.Shared;
using HorseGame.Unified.Generators;
using HorseGame.Shared.Clues;

namespace HorseGame.Tests;

public class ClueGeneratorTests
{
    [Fact]
    public void SuitableGameGenerator_Generates10Levels()
    {
        var generator = new SuitableGameGenerator();
        var game = generator.BuildSuitable();

        Assert.Equal(10, game.Levels.Count);
    }

    [Fact]
    public void NotFirstGenerator_GeneratesCluesForNonWinners()
    {
        var generator = new SuitableGameGenerator();
        var game = generator.BuildSuitable();
        var clueGen = new NotFirstGenerator();

        var clues = clueGen.GetClues(game).ToList();

        Assert.Equal(30, clues.Count);
        Assert.All(clues, c => Assert.IsType<NotFirst>(c));
    }

    [Fact]
    public void TopTwoGenerator_GeneratesCluesForTop2()
    {
        var generator = new SuitableGameGenerator();
        var game = generator.BuildSuitable();
        var clueGen = new TopTwoGenerator();

        var clues = clueGen.GetClues(game).ToList();

        Assert.Equal(20, clues.Count);
        Assert.All(clues, c => Assert.IsType<TopTwo>(c));
    }

    [Fact]
    public void TopThreeGenerator_GeneratesCluesForTop3()
    {
        var generator = new SuitableGameGenerator();
        var game = generator.BuildSuitable();
        var clueGen = new TopThreeGenerator();

        var clues = clueGen.GetClues(game).ToList();

        Assert.Equal(30, clues.Count);
        Assert.All(clues, c => Assert.IsType<TopThree>(c));
    }

    [Fact]
    public void ClueService_GeneratesRandomizedClues()
    {
        var generator = new SuitableGameGenerator();
        var game = generator.BuildSuitable();
        var service = new Unified.Services.ClueService();

        var clues = service.GenerateClues(game);

        Assert.NotEmpty(clues);
        Assert.True(clues.Count > 50);
    }
}
