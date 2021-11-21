using HorseGame.Generator;
using HorseGame.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.ClueGenerator
{
    public class Program
    {
        public static IClue[] GetClues(Game game)
        {
            var generators = new List<IClueGenerator>();
            generators.Add(new OverTakeGenerator());

            var allClues = generators.SelectMany(t => t.GetClues(game)).ToArray();
            return allClues;
        }

        public static void Main(string[] args)
        {
            var game = new SuitableGameGenerator()
                .BuildSuitable();

           var clues = GetClues(game);
        }
    }
}
