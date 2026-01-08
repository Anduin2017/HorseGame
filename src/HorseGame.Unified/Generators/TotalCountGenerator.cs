using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.Unified.Generators
{
    public class TotalCountGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            var champions = new List<string>();
            var seconds = new List<string>();
            var thirds = new List<string>();
            var fourths = new List<string>();

            var horseEvaluator = new HorseEvaluator();
            foreach (var level in game.Levels)
            {
                var positions = new List<PositionMap>();
                positions.Add(new PositionMap
                {
                    Time = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                    HorseName = "Gryffindor"
                });
                positions.Add(new PositionMap
                {
                    Time = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                    HorseName = "Ravenclaw"
                });
                positions.Add(new PositionMap
                {
                    Time = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                    HorseName = "Hufflepuff"
                });
                positions.Add(new PositionMap
                {
                    Time = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds),
                    HorseName = "Slytherin"
                });
                positions = positions.OrderBy(t => t.Time).ToList();

                champions.Add(positions[0].HorseName);
                seconds.Add(positions[1].HorseName);
                thirds.Add(positions[2].HorseName);
                fourths.Add(positions[3].HorseName);
            }

            var horseNames = new[]
            {
                "Gryffindor",
                "Ravenclaw",
                "Hufflepuff",
                "Slytherin"
            };

            foreach (var horseName in horseNames)
            {
                yield return new TotalCount
                {
                    HorseName = horseName,
                    Time = champions.Count(t => t == horseName),
                    Position = "Champion"
                };
                yield return new TotalCount
                {
                    HorseName = horseName,
                    Time = seconds.Count(t => t == horseName),
                    Position = "second"
                };
                yield return new TotalCount
                {
                    HorseName = horseName,
                    Time = thirds.Count(t => t == horseName),
                    Position = "thrid"
                };
                yield return new TotalCount
                {
                    HorseName = horseName,
                    Time = fourths.Count(t => t == horseName),
                    Position = "fourth"
                };
            }
        }
    }
}
