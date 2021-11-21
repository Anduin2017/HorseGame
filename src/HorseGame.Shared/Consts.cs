using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class Consts
    {
        public static int[] PossibleSpeeds = new int[] { 20, 10, 7, 5 };
        public const int DurationForEachSpeed = 1;
        public const int SpeedStagesEachLevel = 5;
        public const int HorseCount = 4;
        public const int TotalLength = 100;
        public const int LevelsCountInAGame = 10;
        public static int[] GradeScoreMatch = new int[] { 3, 2, 1, 0 };
    }
}
