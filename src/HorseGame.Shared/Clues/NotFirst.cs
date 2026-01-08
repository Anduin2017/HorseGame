using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 排除法线索：指明某马不会在特定轮次获得冠军
    /// 用于组合推理，缩小可能性范围
    /// </summary>
    public class NotFirst : IClue
    {
        public string HorseName { get; set; } = string.Empty;

        /// <summary>
        /// Round number, starts from 0
        /// </summary>
        public int Level { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will NOT be the champion in round {Level + 1}.";
        }
    }
}
