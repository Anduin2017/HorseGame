using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 相对关系线索：指明某马的名次会高于另一匹马
    /// 建立马匹间的相对顺序，用于传递推理
    /// </summary>
    public class BetterThan : IClue
    {
        /// <summary>
        /// The horse that finishes ahead
        /// </summary>
        public string BetterHorse { get; set; } = string.Empty;

        /// <summary>
        /// The horse that finishes behind
        /// </summary>
        public string WorseHorse { get; set; } = string.Empty;

        /// <summary>
        /// Round number, starts from 0
        /// </summary>
        public int Level { get; set; }

        public string Print()
        {
            return $"Horse {BetterHorse} will finish ahead of {WorseHorse} in round {Level + 1}.";
        }
    }
}
