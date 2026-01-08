using Xunit;
using HorseGame.Shared;

namespace HorseGame.Tests;

public class EvaluatorTests
{
    [Fact]
    public void HorseEvaluator_CalculatesTimeCorrectly()
    {
        var evaluator = new HorseEvaluator();
        var speeds = new int[] { 10, 10, 10, 10, 10 };

        var time = evaluator.EvaluatorTime(speeds);

        Assert.True(time > 0);
    }

    [Fact]
    public void OvertakeEvaluator_Champion_Gets3Points()
    {
        var evaluator = new OvertakeEvaluator();
        var times = new[] { 10.0, 11.0, 12.0, 13.0 }; // Sorted by time

        var score = evaluator.GetScoreBasedOnTimeChart(times, 10.0); // Fastest

        Assert.Equal(3, score); // Updated: Champion gets 3 points now, not 4
    }

    [Fact]
    public void OvertakeEvaluator_Last_Gets0Points()
    {
        var evaluator = new OvertakeEvaluator();
        var times = new[] { 10.0, 11.0, 12.0, 13.0 };

        var score = evaluator.GetScoreBasedOnTimeChart(times, 13.0); // Slowest

        Assert.Equal(0, score); // Updated: Last place gets 0 points now, not 1
    }
}
