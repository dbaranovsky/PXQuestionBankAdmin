using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.QuestionParserModule.QML;
using Macmillan.PXQBA.Business.QuestionParserModule.Respondus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Tests
{
     [TestClass]
     [DeploymentItem(@"FileForImport\QML")]
   public class QMLQuestionParserTests
    {
        [TestMethod]
        public void Parse_QMLFile_TwelveParsedQuestionsWithoutErros()
        {
            var fileName = "qml_sample.xml";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QMLQuestionPaser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 12);
            Assert.IsFalse(result.FileValidationResults.First().ValidationErrors.Any());

        }

        [TestMethod]
        public void Parse_FileWithThreeQuestion_TenParsedQuestionsWithTwoErrors()
        {
            var fileName = "qml_with_two_unknown_types.xml";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QMLQuestionPaser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count(x => x.IsParsed) == 10);
            Assert.IsTrue(result.FileValidationResults.First().ValidationErrors.Count == 2);
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count(x => !x.IsParsed) == 2);

        }

      
    }
}
