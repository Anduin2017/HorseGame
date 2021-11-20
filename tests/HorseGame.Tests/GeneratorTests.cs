using HorseGame.Generator;
using HorseGame.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HorseGame.Tests
{
    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        public void SpeedsGeneratorTests()
        {
            var generator = new GameGenerator();
            var speeds = generator.GetRandomSpeeds();
            foreach (var possibleSpeed in Consts.PossibleSpeeds)
            {
                var matchedCount = speeds.Where(t => t == possibleSpeed);
                Assert.AreEqual(matchedCount.Count(), Consts.SpeedStagesEachLevel);
            }
        }

        [TestMethod]
        public void LevelGeneratorTests()
        {
            var gameGenerator = new GameGenerator();
            var level = gameGenerator.BuildALevel();

            Assert.AreEqual(level.GryffindorSpeeds.Count(), Consts.SpeedStagesEachLevel);
            Assert.AreEqual(level.HufflepuffSpeeds.Count(), Consts.SpeedStagesEachLevel);
            Assert.AreEqual(level.RavenclawSpeeds.Count(), Consts.SpeedStagesEachLevel);
            Assert.AreEqual(level.SlytherinSpeeds.Count(), Consts.SpeedStagesEachLevel);
        }

        [TestMethod]
        public void GameGeneratorTests()
        {
            var gameGenerator = new GameGenerator();
            var game = gameGenerator.Build();
            Assert.AreEqual(game.Levels.Count(), Consts.LevelsCountInAGame);
            foreach (var level in game.Levels)
            {
                Assert.AreEqual(level.GryffindorSpeeds.Count(), Consts.SpeedStagesEachLevel);
                Assert.AreEqual(level.HufflepuffSpeeds.Count(), Consts.SpeedStagesEachLevel);
                Assert.AreEqual(level.RavenclawSpeeds.Count(), Consts.SpeedStagesEachLevel);
                Assert.AreEqual(level.SlytherinSpeeds.Count(), Consts.SpeedStagesEachLevel);
            }
        }
    }
}
