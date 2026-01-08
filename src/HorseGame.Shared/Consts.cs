using System;

namespace HorseGame.Shared
{
    public class Consts
    {
        public static int[] PossibleSpeeds = new int[] { 40, 20, 14, 10 };
        public const int DurationForEachSpeed = 1;
        public const int SpeedStagesEachLevel = 5;
        public const int HorseCount = 4;
        public const int TotalLength = 100;
        public const int LevelsCountInAGame = 10;
        public static int[] GradeScoreMatch = new int[] { 3, 2, 1, 0 };
    }
}
