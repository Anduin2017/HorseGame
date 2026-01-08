using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 极值线索：指明某马不会是最快的
    /// 排除冠军可能性，但不给出具体名次
    /// </summary>
    public class NotFastest : IClue
    {
        public string HorseName { get; set; } = string.Empty;

        /// <summary>
        /// Round number, starts from 0
        /// </summary>
        public int Level { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will NOT be the fastest in round {Level + 1}.";
        }
    }
}
