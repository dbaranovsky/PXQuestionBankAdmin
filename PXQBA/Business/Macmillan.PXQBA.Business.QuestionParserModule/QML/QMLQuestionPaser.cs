using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QML
{
    public class QMLQuestionPaser : QuestionParserBase
    {
        private readonly string QuestionTypeXpath = "itemmetadata/qmd_itemtype";
        private ValidationResult result;
        private FileValidationResult fileValidationResult;
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase);
        }
          
        public override ValidationResult Parse(string fileName, byte[] file)
        {
            var data = XDocument.Parse(Encoding.UTF8.GetString(file));
            fileValidationResult = new FileValidationResult()
                                   {
                                       FileName = fileName,
                                       Questions = new List<ParsedQuestion>(),
                                       ValidationErrors = new List<string>()
                                   };
            result = new ValidationResult()
                     {
                         FileValidationResults = new List<FileValidationResult>()
                     };

            var itemsXml = new List<XElement>();
            try
            {
                itemsXml = data.Descendants("item").ToList();
            }
            catch (Exception e)
            {
                result.FileValidationResults.Add(new FileValidationResult()
                                                 {
                                                     FileName = fileName,
                                                     Questions = new List<ParsedQuestion>(),
                                                     ValidationErrors = new List<string>()
                                                                        {
                                                                            String.Format("Error during parsing QML Format: {0}", e.Message)
                                                                        }
                                                 });
            }

            foreach (var element in itemsXml)
            {
              ProcessXmlItem(element);
            }
            result.FileValidationResults.Add(fileValidationResult);
            return result;
          
        }

        private bool IsTypeExist(XElement item)
        {
            return
                EnumHelper.GetEnumValues(typeof (QMLType))
                    .Select(x => x.Value)
                    .Contains(item.XPathSelectElement(QuestionTypeXpath).Value);
        }

        private void ProcessXmlItem(XElement item)
        {
            if (IsTypeExist(item))
            {
                fileValidationResult.ValidationErrors.Add(String.Format("Line:{0}: Unknown question type:{1} ", GetLineNumber(item), item.XPathSelectElement(QuestionTypeXpath).Value));
            }
            
            var questionType = (QMLType)EnumHelper.GetItemByDescription(typeof(QMLType), item.XPathSelectElement(QuestionTypeXpath).Value);
            try
            {
                switch (questionType)
                {
                    case QMLType.MultipleChoice:
                        var question = ParseMultiChoiceQuestion(item);
                        question.Type = ParsedQuestionType.MultipleChoice;
                        fileValidationResult.Questions.Add(question);
                        return;
                    default:
                        throw new Exception("QMLQuestionParser: no such question type");
                }
            }
            catch (Exception e)
            {
                fileValidationResult.ValidationErrors.Add(String.Format("Line:{0}: Error during question processing: {1}", GetLineNumber(item), e.Message));
            }
        }

        private int GetLineNumber(XElement item)
        {
           return ((IXmlLineInfo)item).HasLineInfo() ? ((IXmlLineInfo)item).LineNumber : -1;
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
