using Xunit;
using HorseGame.Shared;
using HorseGame.Unified.Services;

namespace HorseGame.Tests;

public class BettingServiceTests
{
    [Fact]
    public void CalculatePayout_Champion_Rounds1To9()
    {
        var service = new BettingService();
        var payout = service.CalculatePayout(10, 1, 5); // 10元 bet, 1st place, round 5

        Assert.Equal(30m, payout); // 3x
    }

    [Fact]
    public void CalculatePayout_RunnerUp_Returns1x()
    {
        var service = new BettingService();
        var payout = service.CalculatePayout(10, 2, 5);

        Assert.Equal(10m, payout); // 1x (return bet)
    }

    [Fact]
    public void CalculatePayout_Third_Returns0Point5x()
    {
        var service = new BettingService();
        var payout = service.CalculatePayout(10, 3, 5);

        Assert.Equal(5m, payout); // 0.5x
    }

    [Fact]
    public void CalculatePayout_Fourth_ReturnsZero()
    {
        var service = new BettingService();
        var payout = service.CalculatePayout(10, 4, 5);

        Assert.Equal(0m, payout);
    }

    [Fact]
    public void CalculatePayout_Round10_SameAsOtherRounds()
    {
        var service = new BettingService();
        var payout = service.CalculatePayout(10, 1, 10); // Round 10

        Assert.Equal(30m, payout); // Still 3x
    }

    [Fact]
    public void CalculateTotalIncome_Round5()
    {
        var service = new BettingService();
        var income = service.CalculateTotalIncome(5);

        Assert.Equal(15m, income); // 1+2+3+4+5 = 15
    }

    [Fact]
    public void CalculateTotalIncome_Round10()
    {
        var service = new BettingService();
        var income = service.CalculateTotalIncome(10);

        Assert.Equal(55m, income); // 1+2+...+10 = 55
    }
}
