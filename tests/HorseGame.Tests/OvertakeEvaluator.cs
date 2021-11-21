using HorseGame.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Tests
{
    [TestClass]
    public class OvertakeEvaluatorTests
    {
        [TestMethod]
        public void TestOvertake()
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

            // S takes H.
            // R takes G.

            var overTakeEvaluator = new OvertakeEvaluator();
            var overTakes = overTakeEvaluator.GetOvertakes(game).ToArray();
            Assert.AreEqual(overTakes.Count(), 2);
            Assert.AreEqual(overTakes[0].Beater, "Ravenclaw");
            Assert.AreEqual(overTakes[0].Previous, "Gryffindor");
            Assert.AreEqual(overTakes[1].Beater, "Slytherin");
            Assert.AreEqual(overTakes[1].Previous, "Hufflepuff");
        }
    }
}
