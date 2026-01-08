using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 马 A 会在第 N 轮跑出第 X 名的成绩。
    /// </summary>
    public class Positioning : IClue
    {
        public string HorseName { get; set; } = string.Empty;

        /// <summary>
        /// Starts from 0.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 1,2,3,4
        /// </summary>
        public int Place { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will run at the {Place}th place in the {Level + 1}th round.";
        }
    }
}
