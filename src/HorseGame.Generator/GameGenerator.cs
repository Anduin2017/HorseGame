using HorseGame.Shared;

namespace HorseGame.Generator
{
    public class GameGenerator
    {
        public Game Build()
        {
            var game = new Game();
            for (int i = 0; i < 10; i++)
            {
                var level = this.BuildLevel();
                game.Levels.Add(level);
            }
            return game;
        }

        public Level BuildLevel()
        {
            var newLevel = new Level();
            return newLevel;
        }
    }
}
