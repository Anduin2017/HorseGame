using HorseGame.Shared;
using HorseGame.Shared.Clues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Unified.Generators
{
    public class PositionMap
    {
        public double Time { get; set; }
        public string HorseName { get; set; } = string.Empty;
    }

    public class FasterGenerator : IClueGenerator
    {
        public IEnumerable<IClue> GetClues(Game game)
        {
            return GetCluesWithoutLimitation(game)
                .Where(t => (t as Faster)?.Level > 3)
                .Where(t => (t as Faster)?.Level != Consts.LevelsCountInAGame - 1);
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
                yield return new Faster
                {
                    Fasterer = positions[0].HorseName,
                    Slower = positions[1].HorseName,
                    Level = index
                };
                yield return new Faster
                {
                    Fasterer = positions[0].HorseName,
                    Slower = positions[2].HorseName,
                    Level = index
                };
                yield return new Faster
                {
                    Fasterer = positions[0].HorseName,
                    Slower = positions[3].HorseName,
                    Level = index
                };
                yield return new Faster
                {
                    Fasterer = positions[1].HorseName,
                    Slower = positions[2].HorseName,
                    Level = index
                };
                yield return new Faster
                {
                    Fasterer = positions[1].HorseName,
                    Slower = positions[3].HorseName,
                    Level = index
                };
                yield return new Faster
                {
                    Fasterer = positions[2].HorseName,
                    Slower = positions[3].HorseName,
                    Level = index
                };
            }
        }
    }
}
