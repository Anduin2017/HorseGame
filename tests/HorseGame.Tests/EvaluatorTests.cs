using Xunit;
using HorseGame.Shared;

namespace HorseGame.Tests;

public class EvaluatorTests
{
    [Fact]
    public void HorseEvaluator_CalculatesTimeCorrectly()
    {
        var evaluator = new HorseEvaluator();
        var speeds = new[] { 10.0, 10.0, 10.0, 10.0, 10.0 };

        var time = evaluator.EvaluatorTime(speeds);

        Assert.True(time > 0);
        Assert.Equal(50.0, time); // 5 segments * 10 = 50
    }

    [Fact]
    public void OvertakeEvaluator_Champion_Gets4Points()
    {
        var evaluator = new OvertakeEvaluator();
        var times = new[] { 10.0, 11.0, 12.0, 13.0 }; // Sorted by time

        var score = evaluator.GetScoreBasedOnTimeChart(times, 10.0); // Fastest

        Assert.Equal(4, score);
    }

    [Fact]
    public void OvertakeEvaluator_Last_Gets1Point()
    {
        var evaluator = new OvertakeEvaluator();
        var times = new[] { 10.0, 11.0, 12.0, 13.0 };

        var score = evaluator.GetScoreBasedOnTimeChart(times, 13.0); // Slowest

        Assert.Equal(1, score);
    }
}
