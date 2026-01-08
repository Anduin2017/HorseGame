using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.Unified.Generators
{
    /// <summary>
    /// Generator for TopTwo clues - range constraint clues for horses finishing in top 2
    /// </summary>
    public class TopTwoGenerator : IClueGenerator
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

                // Get top 2 horses (fastest times)
                var topTwo = times.OrderBy(kv => kv.Value).Take(2).Select(kv => kv.Key).ToList();

                // Generate TopTwo clues for both top horses
                foreach (var horse in topTwo)
                {
                    clues.Add(new TopTwo
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
