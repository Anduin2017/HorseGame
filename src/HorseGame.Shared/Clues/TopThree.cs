
namespace HorseGame.Shared.Clues
{
    /// <summary>
    /// 范围限定线索：指明某马会进入前三名
    /// 比TopTwo弱一些，可以更频繁出现
    /// </summary>
    public class TopThree : IClue
    {
        public string HorseName { get; set; } = string.Empty;

        /// <summary>
        /// Round number, starts from 0
        /// </summary>
        public int Level { get; set; }

        public string Print()
        {
            return $"Horse {HorseName} will finish in top 3 in round {Level + 1}.";
        }
    }
}
