using HorseGame.Shared;

namespace HorseGame.Unified.Generators
{
    public class OverTakeGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var evaluator = new OvertakeEvaluator();
            var results = evaluator.GetOvertakes(game)
                .Where(t => t.Level > 1)
                .Where(t => t.Level != Consts.LevelsCountInAGame - 1);
            foreach(var result in results)
            {
                yield return result;
            }
        }
    }
}
