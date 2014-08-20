using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Business.QuestionParserModule.Respondus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Tests
{
    [TestClass]
    [DeploymentItem(@"FileForImport\QTI")]
    public class QTIQuestionParserTest
    {
        [TestMethod]
        public void Parse_FileWithAllFormate_FiveParsedQuestionsWithOneEssayQuestion()
        {
            var fileName = "qti_all_formats.zip";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QTIQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 7);
            Assert.IsTrue(result.FileValidationResults.First().Questions.Any(x => x.Type == ParsedQuestionType.Essay));

        }

        [TestMethod]
        public void Parse_FileWithOneQuestion_OneParsedQuestions()
        {
            var fileName = "qti_one_question_with_image.zip";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QTIQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 1);
            Assert.IsTrue(result.FileValidationResults.First().Resources.Any());

        }

        [TestMethod]
        public void Parse_FileWithImages_230ParsedQuestionsWithImages()
        {
            var fileName = "qti_with_images.zip";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QTIQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 230);
            Assert.IsTrue(result.FileValidationResults.First().Resources.Any());

        }


        [TestMethod]
        public void Parse_FileWithFiveQuestion_FiveParsedQuestionsWithImages()
        {
            var fileName = "qti_with_images_5_question.zip";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QTIQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 5);
            Assert.IsTrue(result.FileValidationResults.First().Resources.Any());

        }


        [TestMethod]
        public void Parse_FileWithtoutImages_230QuestionsWithoutImages()
        {
            var fileName = "qti_without_images.zip";
            Assert.IsTrue(File.Exists(fileName));
            var parser = new QTIQuestionParser();
            var result = parser.Parse(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Questions.Count == 230);
            Assert.IsFalse(result.FileValidationResults.First().Resources.Any());

        }
    }
}
