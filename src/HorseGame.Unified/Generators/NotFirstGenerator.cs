using System.Collections.Generic;
using System.Linq;
using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.Unified.Generators
{
    /// <summary>
    /// Generator for NotFirst clues - exclusion clues that eliminate potential winners
    /// </summary>
    public class NotFirstGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var clues = new List<IClue>();
            var horseEvaluator = new HorseEvaluator();
            var horses = new[] { "Gryffindor", "Hufflepuff", "Ravenclaw", "Slytherin" };

            for (int levelIndex = 0; levelIndex < game.Levels.Count; levelIndex++)
            {
                var level = game.Levels[levelIndex];

                // Calculate times for this round
                var times = new Dictionary<string, double>
                {
                    ["Gryffindor"] = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                    ["Hufflepuff"] = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                    ["Ravenclaw"] = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                    ["Slytherin"] = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds)
                };

                // Find the winner (fastest time)
                var winner = times.OrderBy(kv => kv.Value).First().Key;

                // Generate NotFirst clues for non-winners
                foreach (var horse in horses.Where(h => h != winner))
                {
                    clues.Add(new NotFirst
                    {
                        HorseName = horse,
                        Level = levelIndex
                    });
                }
            }

            return clues;
        }
    }
}
