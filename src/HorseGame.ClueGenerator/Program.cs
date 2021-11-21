using HorseGame.Generator;
using HorseGame.Shared;

namespace HorseGame.ClueGenerator
{
    public class Program
    {
        public static IClue[] GetClues(Game game)
        {
            var generators = new List<IClueGenerator>();
            generators.Add(new OverTakeGenerator());
            generators.Add(new FasterGenerator());
            generators.Add(new PositioningGenerator());
            generators.Add(new TotalCountGenerator());

            var allClues = generators
                .SelectMany(t => t.GetClues(game))
                .ToArray();
            return allClues;
        }

        public static void Main(string[] args)
        {
            var game = new SuitableGameGenerator()
                .BuildSuitable();

            var clues = GetClues(game)
                 .Select(t => t.Print());

            foreach(var clue in clues)
            {
                Console.WriteLine(clue);
            }
        }
    }
}
