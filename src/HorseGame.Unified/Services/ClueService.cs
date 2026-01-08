using System;
using System.Collections.Generic;
using System.Linq;
using HorseGame.Shared;
using HorseGame.Unified.Generators;

namespace HorseGame.Unified.Services
{
    public class ClueService
    {
        public List<string> GenerateClues(Game game)
        {
            var generators = new List<IClueGenerator>();

            // $"Horse {Beater} will surpass Horse {Previous} with total score after round {Level + 1}.";
            generators.Add(new OverTakeGenerator());

            // $"Horse {Fasterer} will run faster than Horse {Slower} at round {Level + 1}.";
            generators.Add(new FasterGenerator());
            generators.Add(new FasterGenerator());

            // $"Horse {HorseName} will run at the {Place}th place in the {Level + 1}th round.";
            generators.Add(new PositioningGenerator());
            generators.Add(new PositioningGenerator());

            // $"Horse {HorseName} will get totally {Time} times at the {Position} place.";
            generators.Add(new TotalCountGenerator());
            generators.Add(new TotalCountGenerator());

            var allClues = generators
                .SelectMany(t => t.GetClues(game))
                .Select(t => t.Print())
                .ToList();

            // Randomize clues
            var random = new Random();
            return allClues.OrderBy(t => random.Next()).ToList();
        }
    }
}
