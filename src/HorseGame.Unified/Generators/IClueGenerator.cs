using HorseGame.Shared;

namespace HorseGame.Unified.Generators
{
    public interface IClueGenerator
    {
        IEnumerable<IClue> GetClues(Game game);
    }
}
