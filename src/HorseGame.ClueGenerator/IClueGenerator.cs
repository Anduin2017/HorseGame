using HorseGame.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.ClueGenerator
{
    public interface IClueGenerator 
    {
        IEnumerable<IClue> GetClues(Game game);
    }
}
