using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class GameSession
    {
        public Game Game { get; set; } = new Game();
        public List<Player> Players { get; set; } = new List<Player>();
        public Dictionary<string, List<Bet>> PlayerBets { get; set; } = new Dictionary<string, List<Bet>>();
        public int CurrentRound { get; set; } = 0;

        /// <summary>
        /// Calculate player balance after a specific round
        /// </summary>
        public decimal CalculatePlayerBalance(string playerName, int round)
        {
            var player = Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null) return 0;

            decimal balance = player.InitialBalance;

            if (!PlayerBets.ContainsKey(playerName))
                return balance;

            var bets = PlayerBets[playerName].Where(b => b.RoundNumber <= round).OrderBy(b => b.RoundNumber);

            foreach (var bet in bets)
            {
                // Add money at the start of each round (based on README rules)
                balance += bet.RoundNumber; // Round 1 adds 1, Round 2 adds 2, etc.

                // Deduct bet amount
                balance -= bet.BetAmount;

                // Add payout
                balance += bet.Payout;
            }

            return balance;
        }

        /// <summary>
        /// Calculate payout based on horse position
        /// For rounds 1-9: Champion = 3x, Runner-up = 1x, Third = 0.5x, Fourth = 0
        /// For round 10: Based on total score ranking
        /// </summary>
        public decimal CalculatePayout(decimal betAmount, int horsePosition, int roundNumber)
        {
            if (roundNumber < 10)
            {
                // Regular rounds (1-9)
                return horsePosition switch
                {
                    1 => betAmount * 3,  // Champion: 3x
                    2 => betAmount,      // Runner-up: 1x
                    3 => betAmount * 0.5m, // Third: 0.5x
                    _ => 0               // Fourth: 0
                };
            }
            else
            {
                // Round 10: Based on total score
                return horsePosition switch
                {
                    1 => betAmount * 3,
                    2 => betAmount,
                    3 => betAmount * 0.5m,
                    _ => 0
                };
            }
        }
    }
}
