#pragma warning disable IDE0005
#pragma warning disable
﻿namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 马 A 会总共取得 X 次 第 N 名。
    /// </summary>
    public class TotalCount : IClue
    {
        public required string HorseName { get; set; }
        public int Time { get; set; }
        public required string Position { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will get totally {Time} times at the {Position} place.";
        }
    }
}
