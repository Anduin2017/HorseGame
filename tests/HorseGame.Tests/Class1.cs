using HorseGame.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HorseGame.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void HorseEvaluatorTest1()
        {
            var evaluator = new HorseEvaluator();
            var result = evaluator.TimeEvaluator(new List<int>
            {
                20,20,20,20,20
            });
            Assert.AreEqual(result, 5);
        }

        [TestMethod]
        public void HorseEvaluatorTest2()
        {
            var evaluator = new HorseEvaluator();
            var result = evaluator.TimeEvaluator(new List<int>
            {
                20,20,20,20,40
            });
            Assert.AreEqual(result, 4.5);
        }

        [TestMethod]
        public void HorseEvaluatorTest3()
        {
            var evaluator = new HorseEvaluator();
            var result = evaluator.TimeEvaluator(new List<int>
            {
                20,20,10,10,10
            });
            // All stages finished, is at 70, 5 seconds. Remaining 30.
            // Use speed 10 to finish the next 30. Costs 3 seconds. Totally 8.
            Assert.AreEqual(result, 8);
        }
    }
}
