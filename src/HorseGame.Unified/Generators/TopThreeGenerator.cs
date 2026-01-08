using System.Collections.Generic;
using System.Linq;
using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.Unified.Generators
{
    /// <summary>
    /// Generator for TopThree clues - range constraint clues for horses finishing in top 3
    /// </summary>
    public class TopThreeGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var clues = new List<IClue>();
            var horseEvaluator = new HorseEvaluator();

            for (int levelIndex = 0; levelIndex < game.Levels.Count; levelIndex++)
            {
                var level = game.Levels[levelIndex];
                
                // Calculate times and rankings
                var times = new Dictionary<string, double>
                {
                    ["Gryffindor"] = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                    ["Hufflepuff"] = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                    ["Ravenclaw"] = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                    ["Slytherin"] = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds)
                };

                // Get top 3 horses (fastest times)
                var topThree = times.OrderBy(kv => kv.Value).Take(3).Select(kv => kv.Key).ToList();

                // Generate TopThree clues for all top 3 horses
                foreach (var horse in topThree)
                {
                    clues.Add(new TopThree
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
