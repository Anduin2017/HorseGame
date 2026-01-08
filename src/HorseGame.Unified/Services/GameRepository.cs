#pragma warning disable IDE0005
#pragma warning disable
using System.IO;
using System.Text.Json;
using HorseGame.Shared;

namespace HorseGame.Unified.Services
{
    public class GameRepository
    {
        private readonly string gamesDirectory;

        public GameRepository()
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            gamesDirectory = Path.Combine(homeDir, ".horsegame", "games");

            if (!Directory.Exists(gamesDirectory))
            {
                Directory.CreateDirectory(gamesDirectory);
            }
        }

        public void SaveGameSession(GameSession session)
        {
            var filePath = Path.Combine(gamesDirectory, $"{session.Game.GameId}.json");
            var json = JsonSerializer.Serialize(session, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, json);
        }

        public GameSession? LoadGameSession(string gameId)
        {
            var filePath = Path.Combine(gamesDirectory, $"{gameId}.json");
            if (!File.Exists(filePath))
                return null;

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<GameSession>(json);
        }

        public List<Game> ListAllGames()
        {
            var games = new List<Game>();

            if (!Directory.Exists(gamesDirectory))
                return games;

            foreach (var file in Directory.GetFiles(gamesDirectory, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var session = JsonSerializer.Deserialize<GameSession>(json);
                    if (session?.Game != null)
                    {
                        games.Add(session.Game);
                    }
                }
                catch
                {
                    // Skip corrupted files
                }
            }

            return games.OrderByDescending(g => g.CreatedAt).ToList();
        }

        public void DeleteGame(string gameId)
        {
            var filePath = Path.Combine(gamesDirectory, $"{gameId}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
