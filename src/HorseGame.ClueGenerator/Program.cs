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
            generators.Add(new OverTakeGenerator());
            generators.Add(new OverTakeGenerator());
            generators.Add(new FasterGenerator());
            generators.Add(new FasterGenerator());
            generators.Add(new PositioningGenerator());
            generators.Add(new PositioningGenerator());
            generators.Add(new TotalCountGenerator());
            generators.Add(new TotalCountGenerator());

            var allClues = generators
                .SelectMany(t => t.GetClues(game))
                .ToArray();
            return allClues;
        }

        public static void Main(string[] args)
        {
            var gameJson = File.ReadAllText(args.Any() ? args[0] : "game.json");
            var game = System.Text.Json.JsonSerializer.Deserialize<Game>(gameJson) 
                ?? new SuitableGameGenerator().BuildSuitable();

            var clues = GetClues(game)
                 .Select(t => t.Print())
                 .ToArray();

            foreach(var clue in clues)
            {
                Console.WriteLine(clue);
            }

            Console.WriteLine("-----");
            Console.WriteLine("[Press Enter to clear]");
            Console.ReadLine();
            Console.Clear();

            var ran = new Random();
            clues = clues.OrderBy(t => ran.Next()).ToArray();

            foreach(var clue in clues)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("[Press Enter to reveal next clue]");
                Console.WriteLine("-----------------------------------");
                Console.ReadLine();
                Console.Clear();

                Console.WriteLine("-----------------------------------");
                Console.WriteLine("[Confidential] " + clue);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("[Press Enter to hide]");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}
