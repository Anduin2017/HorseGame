using HorseGame.Shared;

namespace HorseGame.Generator
{
    public class SuitableGameGenerator
    {
        private readonly GameGenerator gameGenerator;
        private readonly OvertakeEvaluator overtakeEvaluator;

        public SuitableGameGenerator()
        {
            this.gameGenerator = new GameGenerator();
            this.overtakeEvaluator = new OvertakeEvaluator();
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

            var existsOvertakesAtFinal = overtakes.Count(t => t.Level == Consts.LevelsCountInAGame - 1) > 2;

            return manyOvertakes && existsOvertakesAtFinal;
        }
    }
}
