using HorseGame.Shared;
using HorseGame.Shared.Clues;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Tests
{
    [TestClass]
    public class ClueGeneratorTests
    {
        [TestMethod]
        public void TestClue()
        {
            var game = new Game();

            // G,R,H,S
            // 3,2,1,0
            game.Levels.Add(new Level
            {
                GryffindorSpeeds = new List<int> { 20, 20, 20, 20 },
                RavenclawSpeeds = new List<int> { 20, 20, 20, 10 },
                HufflepuffSpeeds = new List<int> { 20, 20, 20, 7 },
                SlytherinSpeeds = new List<int> { 20, 20, 20, 5 },
            });


            // R+3 = 5
            // S+2 = 2
            // G+1 = 4
            // H+0 = 1

            game.Levels.Add(new Level
            {
                RavenclawSpeeds = new List<int> { 20, 20, 20, 20 },
                SlytherinSpeeds = new List<int> { 20, 20, 20, 10 },
                GryffindorSpeeds = new List<int> { 20, 20, 20, 7 },
                HufflepuffSpeeds = new List<int> { 20, 20, 20, 5 },
            });

            // R,G,S,H
            // 5,4,2,1

            // Add more levels to satisfy filters
            for (int i = 0; i < 10; i++)
            {
                game.Levels.Add(new Level
                {
                    GryffindorSpeeds = new List<int> { 20, 20, 20, 20 },
                    RavenclawSpeeds = new List<int> { 20, 20, 20, 10 },
                    HufflepuffSpeeds = new List<int> { 20, 20, 20, 7 },
                    SlytherinSpeeds = new List<int> { 20, 20, 20, 5 },
                });
            }

            var clues = HorseGame.ClueGenerator.Program.GetClues(game);
            var fasterClues = clues.OfType<Faster>().ToArray();
            Assert.AreEqual("Gryffindor", fasterClues[0].Fasterer);
            Assert.AreEqual("Ravenclaw", fasterClues[0].Slower);

            Assert.AreEqual("Gryffindor", fasterClues[6].Fasterer);
            Assert.AreEqual("Ravenclaw", fasterClues[6].Slower);
        }
    }
}
