#pragma warning disable IDE0005
#pragma warning disable
﻿using System;

namespace HorseGame.Shared
{
    public class HorseEvaluator
    {
        public double EvaluatorTime(IList<int> speeds)
        {
            int gone = 0;
            double timeElapsed = 0;
            foreach (var speed in speeds)
            {
                var locationIfGoWithFullSpeed = gone + speed * Consts.DurationForEachSpeed;
                if (locationIfGoWithFullSpeed < Consts.TotalLength)
                {
                    gone = locationIfGoWithFullSpeed;
                    timeElapsed += Consts.DurationForEachSpeed;
                }
                else
                {
                    var remainingRoad = Consts.TotalLength - gone;
                    var timeRequiredToFinish = (double)remainingRoad / speed;
                    timeElapsed += timeRequiredToFinish;
                    return timeElapsed;
                }
            }

            var remainingRoadAtLast = Consts.TotalLength - gone;
            var lastSpeed = speeds.Last();
            var timeRequiredToFinishAtLast = (double)remainingRoadAtLast / lastSpeed;
            timeElapsed += timeRequiredToFinishAtLast;
            return timeElapsed;
        }
    }
}
