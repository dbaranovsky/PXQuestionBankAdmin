using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QML
{
    public class QMLQuestionPaser : QuestionParserBase
    {
        private readonly string QuestionTypeXpath = "itemmetadata/qmd_itemtype";
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase);
        }

        public override ValidationResult Parse(string fileName, byte[] file)
        {
            var data = XDocument.Parse(Encoding.UTF8.GetString(file));
            var itemsXml = data.Descendants("item").ToList();
            return new ValidationResult()
            {
                FileValidationResults =
                    new List<FileValidationResult>()
                    {
                        new FileValidationResult()
                        {
                            Questions = (itemsXml.Where(IsTypeExist).Select(ConvertXmlItemToQuestion)).ToList()
                        }
                    }
            };
        }

        private bool IsTypeExist(XElement item)
        {

            return
                EnumHelper.GetEnumValues(typeof (QMLType))
                    .Select(x => x.Value)
                    .Contains(item.XPathSelectElement(QuestionTypeXpath).Value);
        }

        private ParsedQuestion ConvertXmlItemToQuestion(XElement item)
        {
            var questionType = (QMLType)EnumHelper.GetItemByDescription(typeof(QMLType), item.XPathSelectElement(QuestionTypeXpath).Value);
            switch (questionType)
            {
              case QMLType.MultipleChoice:
                    return ParseMultiChoiceQuestion(item);
              default:
                    throw new Exception("QMLQuestionPaser: no such question type");
            }
        }

        private ParsedQuestion ParseMultiChoiceQuestion(XElement item)
        {
            var parsedQuestion = new ParsedQuestion();
            parsedQuestion.Title = parsedQuestion.Text = item.Attribute("title").Value;

            var answers = item.Descendants("response_label");
            var answersFeedBack = item.Descendants("itemfeedback");
            var correctAnswers = item.Descendants("respcondition").Where(x => x.Elements("setvar").Any() && x.Element("setvar").Value == "1");
            parsedQuestion.Choices = answers.Select(x => ProccessAnswer(x, answersFeedBack, correctAnswers)).ToList();

            return parsedQuestion;
        }

        private ParsedQuestionChoice ProccessAnswer(XElement answer, IEnumerable<XElement> answersFeedBack, IEnumerable<XElement> correctAnswers)
        {
            var choice = new ParsedQuestionChoice();

            var mattext = answer.Descendants("mattext").FirstOrDefault();
            choice.Text = mattext == null ? string.Empty : mattext.Value;

            if (!answersFeedBack.Any())
            {
                return choice;
            }

            var feedBackItem = answersFeedBack.FirstOrDefault(x => x.Attribute("ident").Value.Contains(choice.Text));
            choice.Feedback = feedBackItem == null ? string.Empty : feedBackItem.Value;
            
            if (!correctAnswers.Any())
            {
                return choice;
            }
            choice.IsCorrect = correctAnswers.Select(x => x.Attribute("title").Value).Any(x=> x.Contains(choice.Text));
           
            return choice;
        }

    }
}
