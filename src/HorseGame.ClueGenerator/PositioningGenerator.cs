using HorseGame.Shared;
using HorseGame.Shared.Clues;

namespace HorseGame.ClueGenerator
{
    public class PositioningGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            return GetCluesWithoutLimitation(game)
                .Where(t => (t as Positioning)?.Level > 1)
                .Where(t => (t as Positioning)?.Level != Consts.LevelsCountInAGame - 1);
        }

        public IEnumerable<IClue> GetCluesWithoutLimitation(Game game)
        {
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

                var index = game.Levels.IndexOf(level);
                yield return new Positioning
                {
                    HorseName = positions[1].HorseName,
                    Place = 2,
                    Level = index
                };
                yield return new Positioning
                {
                    HorseName = positions[2].HorseName,
                    Place = 3,
                    Level = index
                };
                yield return new Positioning
                {
                    HorseName = positions[3].HorseName,
                    Place = 4,
                    Level = index
                };
            }
        }
    }
}
