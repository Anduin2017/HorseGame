using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class Game
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public string GameName { get; set; } = "New Game";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<Level> Levels { get; set; } = new List<Level>();
        public List<string> Clues { get; set; } = new List<string>();
    }
}
