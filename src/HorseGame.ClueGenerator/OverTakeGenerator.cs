using HorseGame.Shared;

namespace HorseGame.ClueGenerator
{
    public class OverTakeGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var evaluator = new OvertakeEvaluator();
            var results = evaluator.GetOvertakes(game);
            foreach(var result in results)
            {
                yield return result;
            }
        }
    }
}
