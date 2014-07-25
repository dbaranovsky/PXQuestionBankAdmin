using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QML
{
    public class QMLQuestionPaser : QuestionParserBase
    {
        private const string QuestionTypeXpath = "itemmetadata/qmd_itemtype";
        private FileValidationResult fileValidationResult;
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase);
        }
          
        public override ValidationResult Parse(string fileName, byte[] file)
        {
            var data = XDocument.Parse(Encoding.UTF8.GetString(file), LoadOptions.SetLineInfo);
            fileValidationResult = new FileValidationResult()
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
                                                                            String.Format("File: {0}, Line: 1 - Error during opening QML Format", fileName)
                                                                        }
                                                 });
            }

            foreach (var element in itemsXml)
            {
              ProcessXmlItem(element, fileName);
            }
            Result.FileValidationResults.Add(fileValidationResult);
            return Result;
          
        }

        private static bool IsTypeExist(XElement item)
        {
            return
                EnumHelper.GetEnumValues(typeof (QMLType))
                    .Select(x => x.Value)
                    .Contains(item.XPathSelectElement(QuestionTypeXpath).Value);
        }

        private void ProcessXmlItem(XElement item, string fileName)
        {
            if (!IsTypeExist(item))
            {
                fileValidationResult.ValidationErrors.Add(String.Format("File: {0}, Line: {1} - Unknown question type: {2} ", fileName, GetLineNumber(item), item.XPathSelectElement(QuestionTypeXpath).Value));
                fileValidationResult.Questions.Add(new ParsedQuestion(){IsParsed = false});
                return;
            }
            
            var questionType = (QMLType)EnumHelper.GetItemByDescription(typeof(QMLType), item.XPathSelectElement(QuestionTypeXpath).Value);
           var question = ParseMultiChoiceQuestion(item);
            try
            {
                switch (questionType)
                {
                    case QMLType.MultipleChoice:
                         question = ParseMultiChoiceQuestion(item);
                        question.Type = ParsedQuestionType.MultipleChoice;
                        question.IsParsed = true;
                        fileValidationResult.Questions.Add(question);
                        return;
                    default:
                        throw new Exception("QMLQuestionParser: no such question type");
                }
            }
            catch (Exception e)
            {
                fileValidationResult.ValidationErrors.Add(String.Format("File: {0}, Line: {1} - Error during question processing", fileName, GetLineNumber(item)));
                question.IsParsed = false;
         
            }

          fileValidationResult.Questions.Add(question);
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
            choice.Id = answer.Attribute(XmlConsts.IdAttrName) == null ||
                        string.IsNullOrEmpty(answer.Attribute(XmlConsts.IdAttrName).Value)
                ? choice.Text
                : answer.Attribute(XmlConsts.IdAttrName).Value;

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
