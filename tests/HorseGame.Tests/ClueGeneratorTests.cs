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
        public void TestNoCrash()
        {
            HorseGame.ClueGenerator.Program.Main(Array.Empty<string>());
        }
    }
}
