using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Macmillan.PXQBA.Business.QuestionParserModule.QML;
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Business.QuestionParserModule.Respondus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Tests
{
    [TestClass]
    [DeploymentItem(@"FileForImport", "TestData")]
    public class QuestionParserBaseTests
    {
        private List<QuestionParserBase> parsers;
        [TestInitialize]
        public void TestInitialize()
        {
             parsers = new List<QuestionParserBase>()
                          {
                              new QMLQuestionPaser(),
                              new QTIQuestionParser(),
                              new RespondusQuestionParser()
                          };
        }

        [TestMethod]
        public void Recognize_FileNameOfAllFormatsPassed_AllFormatsShouldBeRecognized()
        {
            Assert.IsTrue(parsers.Any(x => x.Recognize("qml.xml")));
            Assert.IsTrue(parsers.Any(x => x.Recognize("qti.zip")));
            Assert.IsTrue(parsers.Any(x => x.Recognize("respondus.txt")));
            Assert.IsFalse(parsers.Any(x => x.Recognize("test.avi")));

            Assert.IsInstanceOfType(parsers.First(x => x.Recognize("qml.xml")), typeof(QMLQuestionPaser));
            Assert.IsInstanceOfType(parsers.First(x => x.Recognize("qti.zip")), typeof(QTIQuestionParser));
            Assert.IsInstanceOfType(parsers.First(x => x.Recognize("respondus.txt")), typeof(RespondusQuestionParser));
        }
    }
}
