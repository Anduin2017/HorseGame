using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.Unified.Generators
{
    /// <summary>
    /// Generator for BetterThan clues - relative ranking clues between horses
    /// </summary>
    public class BetterThanGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var clues = new List<IClue>();
            var horseEvaluator = new HorseEvaluator();

            for (int levelIndex = 0; levelIndex < game.Levels.Count; levelIndex++)
            {
                var level = game.Levels[levelIndex];

                // Calculate times
                var times = new Dictionary<string, double>
                {
                    ["Gryffindor"] = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                    ["Hufflepuff"] = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                    ["Ravenclaw"] = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                    ["Slytherin"] = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds)
                };

                // Sort by time (ascending = better)
                var sorted = times.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();

                // Generate BetterThan clues for adjacent pairs
                // This creates a chain: 1st > 2nd, 2nd > 3rd, 3rd > 4th
                for (int i = 0; i < sorted.Count - 1; i++)
                {
                    clues.Add(new BetterThan
                    {
                        BetterHorse = sorted[i],
                        WorseHorse = sorted[i + 1],
                        Level = levelIndex
                    });
                }
            }

            return clues;
        }
    }
}
