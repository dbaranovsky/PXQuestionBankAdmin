using System;
using System.IO;
using System.Linq;
using Macmillan.PXQBA.Business.QuestionParserModule.Respondus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Tests
{
    [TestClass]
    [DeploymentItem(@"FileForImport\Respondus")]
    public class RespondusQuestionParserTests
    {
        [TestMethod]
        public void Parse_FileWithTwoQuestion_TwoParsedQuestions()
        {
            var fileName = "two_questions.txt";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new RespondusQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 2);

        }

        [TestMethod]
        public void Parse_FileWithThreeQuestion_ThreeParsedQuestions()
        {
            var fileName = "three_questions.txt";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new RespondusQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 3);

        }

        [TestMethod]
        public void Parse_FileWithFiveQuestion_FiveParsedQuestions()
        {
            var fileName = "five_questions.txt";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new RespondusQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 5);

        }
    }
}
