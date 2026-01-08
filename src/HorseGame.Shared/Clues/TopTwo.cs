using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 范围限定线索：指明某马会进入前两名
    /// 提供模糊的范围信息，需要结合其他线索推理
    /// </summary>
    public class TopTwo : IClue
    {
        public string HorseName { get; set; } = string.Empty;

        /// <summary>
        /// Round number, starts from 0
        /// </summary>
        public int Level { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will finish in top 2 in round {Level + 1}.";
        }
    }
}
