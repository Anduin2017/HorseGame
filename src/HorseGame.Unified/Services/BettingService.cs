using System;
using HorseGame.Shared;

namespace HorseGame.Unified.Services
{
    /// <summary>
    /// Service for betting calculations and payout logic
    /// </summary>
    public class BettingService
    {
        /// <summary>
        /// Calculate payout based on horse position and round number
        /// </summary>
        /// <param name="betAmount">Original bet amount</param>
        /// <param name="position">Horse finish position (1-4)</param>
        /// <param name="roundNumber">Round number (1-10)</param>
        /// <returns>Payout amount</returns>
        public decimal CalculatePayout(decimal betAmount, int position, int roundNumber)
        {
            if (roundNumber < 10)
            {
                // Rounds 1-9: Based on race position
                return position switch
                {
                    1 => betAmount * 3,      // Champion: 3x
                    2 => betAmount,          // Runner-up: 1x (return bet)
                    3 => betAmount * 0.5m,   // Third: 0.5x
                    _ => 0                   // Fourth: nothing
                };
            }
            else
            {
                // Round 10: Based on total score ranking (same payout structure)
                return position switch
                {
                    1 => betAmount * 3,
                    2 => betAmount,
                    3 => betAmount * 0.5m,
                    _ => 0
                };
            }
        }

        /// <summary>
        /// Calculate round income (1元 for round 1, 2元 for round 2, etc.)
        /// </summary>
        public decimal CalculateRoundIncome(int round)
        {
            return round;
        }

        /// <summary>
        /// Calculate total income from rounds 1 through specified round
        /// </summary>
        public decimal CalculateTotalIncome(int upToRound)
        {
            decimal total = 0;
            for (int r = 1; r <= upToRound; r++)
            {
                total += r;
            }
            return total;
        }
    }
}
