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
            Console.WriteLine($"Overtakes: {overtakes.Count()}");
            return overtakes.Count() > 18;
        }
    }
}
