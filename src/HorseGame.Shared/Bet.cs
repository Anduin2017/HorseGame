
namespace HorseGame.Shared
{
    public class Bet
    {
        public string PlayerName { get; set; } = string.Empty;
        public int RoundNumber { get; set; }
        public string HorseName { get; set; } = string.Empty;
        public decimal BetAmount { get; set; }
        public decimal Payout { get; set; }
    }
}
