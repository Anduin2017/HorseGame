#pragma warning disable IDE0005
#pragma warning disable
﻿using System;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 马 A 会在第 N 轮中，总成绩超过马 B。
    /// </summary>
    public class OverTake : IClue
    {
        /// <summary>
        /// Start from 0.
        /// </summary>
        public int Level { get; set; }

        public string Previous { get; set; } = string.Empty;
        public string Beater { get; set; } = string.Empty;

        public string Print()
        {
            return $"Horse {Beater} will surpass Horse {Previous} with total score after round {Level + 1}.";
        }
    }
}
