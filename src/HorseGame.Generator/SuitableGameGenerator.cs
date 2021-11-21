using HorseGame.Shared;

namespace HorseGame.Generator
{
    public class SuitableGameGenerator
    {
        private readonly GameGenerator gameGenerator;
        private readonly OvertakeEvaluator overtakeEvaluator;
        private readonly HorseEvaluator horseEvaluator;

        public SuitableGameGenerator()
        {
            this.gameGenerator = new GameGenerator();
            this.overtakeEvaluator = new OvertakeEvaluator();
            this.horseEvaluator = new HorseEvaluator();
        }

        public Game BuildSuitable()
        {
            while (true)
            {
                var newGame = this.gameGenerator.Build();
                if (IsGameSuitable(newGame))
                {
                    return newGame;
                }
            }
        }

        private bool IsGameSuitable(Game game)
        {
            var overtakes = this.overtakeEvaluator.GetOvertakes(game).ToArray();
            var manyOvertakes = overtakes.Count() > 18;

            var interestingFinal = overtakes.Count(t => t.Level == Consts.LevelsCountInAGame - 1) > 2;

            var manyOverTakesLater = overtakes.Count(t => t.Level > 5) > 9;
            var notSameScoreFinally = this.SameSore(game);

            return manyOvertakes && interestingFinal && manyOverTakesLater && notSameScoreFinally;
        }

        private bool SameSore(Game game)
        {
            int gryffindorScore = 0;
            int ravenclawScore = 0;
            int hufflepuffScore = 0;
            int slytherinScore = 0;
            foreach (var level in game.Levels)
            {

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

                gryffindorScore += this.overtakeEvaluator.GetScoreBasedOnTimeChart(scores, gryffindorTime);
                ravenclawScore += this.overtakeEvaluator.GetScoreBasedOnTimeChart(scores, ravenclawTime);
                hufflepuffScore += this.overtakeEvaluator.GetScoreBasedOnTimeChart(scores, hufflepuffTime);
                slytherinScore += this.overtakeEvaluator.GetScoreBasedOnTimeChart(scores, slytherinsTime);
            }

            return gryffindorScore != ravenclawScore && 
                ravenclawScore  != hufflepuffScore && 
                hufflepuffScore != slytherinScore;
        }
    }
}
