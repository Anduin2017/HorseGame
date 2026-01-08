using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Shared
{
    public class Player
    {
        public string PlayerName { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; } = 12;
        public List<int> PurchasedClueIndices { get; set; } = new List<int>(); // Indices of purchased clues
    }
}
