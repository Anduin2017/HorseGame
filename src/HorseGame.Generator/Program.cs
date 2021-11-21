using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var generator = new SuitableGameGenerator();
            var game = generator.BuildSuitable();
            var gameJson = System.Text.Json.JsonSerializer.Serialize(game);
            File.WriteAllText("game.json", gameJson);
        }
    }
}
