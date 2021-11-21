using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class OverTake
    {
        public string Previous { get; set; } = string.Empty;
        public string Beater { get; set; } = string.Empty;
    }

    public class Facing
    {
        public List<string> GryffindorFacing { get; set; } = new List<string>();
        public List<string> RavenclawFacing { get; set; } = new List<string>();
        public List<string> HufflepuffFacing { get; set; } = new List<string>();
        public List<string> SlytherinFacing { get; set; } = new List<string>();
    }

    public class OvertakeEvaluator
    {
        private readonly HorseEvaluator horseEvaluator;

        public OvertakeEvaluator()
        {
            this.horseEvaluator = new HorseEvaluator();
        }

        public IEnumerable<OverTake> GetOvertakes(Game game)
        {
            int gryffindorScore = 0;
            int ravenclawScore = 0;
            int hufflepuffScore = 0;
            int slytherinScore = 0;
            foreach (var level in game.Levels)
            {
                var previousFacing = GetFacing(gryffindorScore, ravenclawScore, hufflepuffScore, slytherinScore);

                var gryffindorTime = this.horseEvaluator.EvaluatorTime(level.GryffindorSpeeds);
                var ravenclawTime = this.horseEvaluator.EvaluatorTime(level.RavenclawSpeeds);
                var hufflepuffTime = this.horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds);
                var slytherinsTime = this.horseEvaluator.EvaluatorTime(level.SlytherinSpeeds);

                var scores = new List<double>
                {
                    gryffindorTime,
                    ravenclawTime,
                    hufflepuffTime,
                    slytherinsTime
                }.OrderBy(t => t).ToArray();

                gryffindorScore += GetScoreBasedOnTimeChart(scores, gryffindorTime);
                ravenclawScore += GetScoreBasedOnTimeChart(scores, ravenclawTime);
                hufflepuffScore += GetScoreBasedOnTimeChart(scores, hufflepuffTime);
                slytherinScore += GetScoreBasedOnTimeChart(scores, slytherinsTime);

                var newFacing = GetFacing(gryffindorScore, ravenclawScore, hufflepuffScore, slytherinScore);
                var overtakes = GetOvertakesByLevel(previousFacing, newFacing);
                if (game.Levels.IndexOf(level) == 0)
                {
                    continue;
                }
                foreach (var overtake in overtakes)
                {
                    yield return overtake;
                }
            }
        }

        private Facing GetFacing(int gryffindorScore, int ravenclawScore, int hufflepuffScore, int slytherinScore)
        {
            var facing = new Facing();
            if (ravenclawScore > gryffindorScore) facing.GryffindorFacing.Add("Ravenclaw");
            if (hufflepuffScore > gryffindorScore) facing.GryffindorFacing.Add("Hufflepuff");
            if (slytherinScore > gryffindorScore) facing.GryffindorFacing.Add("Slytherin");

            if (gryffindorScore > ravenclawScore) facing.RavenclawFacing.Add("Gryffindor");
            if (hufflepuffScore > ravenclawScore) facing.RavenclawFacing.Add("Hufflepuff");
            if (slytherinScore > ravenclawScore) facing.RavenclawFacing.Add("Slytherin");

            if (gryffindorScore > hufflepuffScore) facing.HufflepuffFacing.Add("Gryffindor");
            if (ravenclawScore > hufflepuffScore) facing.HufflepuffFacing.Add("Ravenclaw");
            if (slytherinScore > hufflepuffScore) facing.HufflepuffFacing.Add("Slytherin");

            if (gryffindorScore > slytherinScore) facing.SlytherinFacing.Add("Gryffindor");
            if (ravenclawScore > slytherinScore) facing.SlytherinFacing.Add("Ravenclaw");
            if (hufflepuffScore > slytherinScore) facing.SlytherinFacing.Add("Hufflepuff");

            return facing;
        }

        private int GetScoreBasedOnTimeChart(double[] timeChart, double myTime)
        {
            var index = timeChart.ToList().IndexOf(myTime);
            return Consts.GradeScoreMatch.OrderByDescending(t => t).ToArray()[index];
        }

        private List<OverTake> GetOvertakesByLevel(Facing previousFacing, Facing newFacing)
        {
            var allOvertakes = new List<OverTake>();
            var gryffindorOverTakes = newFacing
                .GryffindorFacing
                .Except(previousFacing.GryffindorFacing)
                .Select(t => new OverTake
                {
                    Previous = "Gryffindor",
                    Beater = t
                });
            var ravenclawOvertakes = newFacing
                .RavenclawFacing
                .Except(previousFacing.RavenclawFacing)
                .Select(t => new OverTake
                {
                    Previous = "Ravenclaw",
                    Beater = t
                });
            var hufflepuffOvertakes = newFacing
                .HufflepuffFacing
                .Except(previousFacing.HufflepuffFacing)
                .Select(t => new OverTake
                {
                    Previous = "Hufflepuff",
                    Beater = t
                });
            var slytherinOvertakes = newFacing
                .SlytherinFacing
                .Except(previousFacing.SlytherinFacing)
                .Select(t => new OverTake
                {
                    Previous = "Slytherin",
                    Beater = t
                });
            allOvertakes.AddRange(gryffindorOverTakes);
            allOvertakes.AddRange(ravenclawOvertakes);
            allOvertakes.AddRange(hufflepuffOvertakes);
            allOvertakes.AddRange(slytherinOvertakes);
            return allOvertakes;
        }
    }
}
