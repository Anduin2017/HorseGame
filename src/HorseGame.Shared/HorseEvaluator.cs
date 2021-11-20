using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class HorseEvaluator
    {
        public double TimeEvaluator(IList<int> speeds)
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
            var timeRequiredToFinishAtLast = remainingRoadAtLast / lastSpeed;
            timeElapsed += timeRequiredToFinishAtLast;
            return timeElapsed;
        }
    }
}
