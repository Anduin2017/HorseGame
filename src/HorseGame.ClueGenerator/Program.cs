using HorseGame.Generator;
using HorseGame.Shared;

namespace HorseGame.ClueGenerator
{
    public class Program
    {
        public static IClue[] GetClues(Game game)
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
                .ToArray();
            return allClues;
        }

        public static void Main(string[] args)
        {
            var path = args.Any() ? args[0] : "game.json";
            Game game;
            if (File.Exists(path))
            {
                var gameJson = File.ReadAllText(path);
                game = System.Text.Json.JsonSerializer.Deserialize<Game>(gameJson)
                    ?? new SuitableGameGenerator().BuildSuitable();
            }
            else
            {
                game = new SuitableGameGenerator().BuildSuitable();
            }

            var clues = GetClues(game)
                 .Select(t => t.Print())
                 .ToArray();

            foreach(var clue in clues)
            {
                Console.WriteLine(clue);
            }

            Console.WriteLine("-----");
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("[Press Enter to clear]");
                Console.ReadLine();
                Console.Clear();
            }

            var ran = new Random();
            clues = clues.OrderBy(t => ran.Next()).ToArray();

            foreach(var clue in clues)
            {
                if (!Console.IsInputRedirected)
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
                else
                {
                    Console.WriteLine("[Confidential] " + clue);
                }
            }
        }
    }
}
