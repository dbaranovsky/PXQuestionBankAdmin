using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;

using NUnit.Framework;

namespace Bfw.Common.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void CorrectWordCount()
        {
            //arrange
            var sevenWords = "I am a loud and angry lout";

            //act
            var words = sevenWords.WordCount();

            //assert
            Assert.AreEqual(7, words);
        }
    }
}
