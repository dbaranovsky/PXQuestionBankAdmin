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
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QML
{
    public class QMLQuestionPaser : QuestionParserBase
    {
        private const string QuestionTypeXpath = "itemmetadata/qmd_itemtype";
        private FileValidationResult _fileValidationResult;
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase);
        }
          
        public override ValidationResult Parse(string fileName, byte[] file)
        {
            var data = XDocument.Parse(Encoding.UTF8.GetString(file));
            _fileValidationResult = new FileValidationResult()
                                   {
                                       FileName = fileName,
                                       Questions = new List<ParsedQuestion>(),
                                       ValidationErrors = new List<string>()
                                   };
            Result = new ValidationResult()
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
                Result.FileValidationResults.Add(new FileValidationResult()
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
            Result.FileValidationResults.Add(_fileValidationResult);
            return Result;
          
        }

        private static bool IsTypeExist(XElement item)
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
                _fileValidationResult.ValidationErrors.Add(String.Format("Line:{0}: Unknown question type:{1} ", GetLineNumber(item), item.XPathSelectElement(QuestionTypeXpath).Value));
            }
            
            var questionType = (QMLType)EnumHelper.GetItemByDescription(typeof(QMLType), item.XPathSelectElement(QuestionTypeXpath).Value);
            try
            {
                switch (questionType)
                {
                    case QMLType.MultipleChoice:
                        var question = ParseMultiChoiceQuestion(item);
                        question.Type = ParsedQuestionType.MultipleChoice;
                        _fileValidationResult.Questions.Add(question);
                        return;
                    default:
                        throw new Exception("QMLQuestionParser: no such question type");
                }
            }
            catch (Exception e)
            {
                _fileValidationResult.ValidationErrors.Add(String.Format("Line:{0}: Error during question processing: {1}", GetLineNumber(item), e.Message));
            }
        }

        private int GetLineNumber(XElement item)
        {
           return ((IXmlLineInfo)item).HasLineInfo() ? ((IXmlLineInfo)item).LineNumber : -1;
        }

        private ParsedQuestion ParseMultiChoiceQuestion(XElement item)
        {
            var parsedQuestion = new ParsedQuestion();
            parsedQuestion.Title = parsedQuestion.Text = item.Attribute(XmlConsts.TitleAttribute).Value;

            var answers = item.Descendants(XmlConsts.ResponseLabelName);
            var answersFeedBack = item.Descendants(XmlConsts.FeedBackElementName);
            var correctAnswers = item.Descendants(XmlConsts.RepsonseVariableName).Where(x => x.Elements(XmlConsts.SetVarElementName).Any() && x.Element(XmlConsts.SetVarElementName).Value == "1");
            parsedQuestion.Choices = answers.Select(x => ProccessAnswer(x, answersFeedBack, correctAnswers)).ToList();

            return parsedQuestion;
        }

        private ParsedQuestionChoice ProccessAnswer(XElement answer, IEnumerable<XElement> answersFeedBack, IEnumerable<XElement> correctAnswers)
        {
            var choice = new ParsedQuestionChoice();

            var mattext = answer.Descendants(XmlConsts.MattextName).FirstOrDefault();
            choice.Text = mattext == null ? string.Empty : mattext.Value;

            if (!answersFeedBack.Any())
            {
                return choice;
            }

            var feedBackItem = answersFeedBack.FirstOrDefault(x => x.Attribute(XmlConsts.IdAttrName).Value.Contains(choice.Text));
            choice.Feedback = feedBackItem == null ? string.Empty : feedBackItem.Value;
            
            if (!correctAnswers.Any())
            {
                return choice;
            }
            choice.IsCorrect = correctAnswers.Select(x => x.Attribute(XmlConsts.TitleAttribute).Value).Any(x=> x.Contains(choice.Text));
           
            return choice;
        }

    }
}
