using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 马 A 会在第 N 轮比马 B 快。
    /// </summary>
    public class Faster : IClue
    {
        /// <summary>
        /// Start from 0.
        /// </summary>
        public int Level { get; set; }

        public string Slower { get; set; } = string.Empty;
        public string Fasterer { get; set; } = string.Empty;

        public string Print()
        {
            return $"Horse {Fasterer} will run faster than Horse {Slower} at round {Level + 1}.";
        }
    }
}
