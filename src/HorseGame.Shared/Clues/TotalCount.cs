using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 马 A 会总共取得 X 次 第 N 名。
    /// </summary>
    public class TotalCount : IClue
    {
        public string HorseName { get; set; }
        public int Time { get; set; }
        public string Position { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will get totally {Time} times at the {Position} place.";
        }
    }
}
