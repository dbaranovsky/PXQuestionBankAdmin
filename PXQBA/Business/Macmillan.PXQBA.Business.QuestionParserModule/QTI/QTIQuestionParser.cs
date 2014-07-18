using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Business.QuestionParserModule.QML;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QTI
{
    public class QTIQuestionParser : QuestionParserBase
    {
        private readonly string QuestionTypeXpath = "itemmetadata/qmd_itemtype";
        private ValidationResult result;
        private FileValidationResult fileValidationResult;
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QTI), StringComparison.CurrentCultureIgnoreCase);
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
                result.FileValidationResults.Add(new FileValidationResult
                                                 {
                                                     FileName = fileName,
                                                     Questions = new List<ParsedQuestion>(),
                                                     ValidationErrors = new List<string>
                                                                        {
                                                                            String.Format(
                                                                                "Error during parsing QML Format: {0}",
                                                                                e.Message)
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

    

        private void ProcessXmlItem(XElement item)
        {
            var parsedQuestion = new ParsedQuestion();
            parsedQuestion.Title = item.Attribute("title").Value;
            parsedQuestion.Text = item.Element("presentation").Descendants("mattext").First().Value;
            var itemFeedBacks = item.Descendants("itemfeedback");
            parsedQuestion.Feedback = itemFeedBacks.All(x => x.Attribute("ident").Value == "general")
                ? string.Empty
                : itemFeedBacks.Single(x => x.Attribute("ident").Value == "general").Value;


            if (item.Elements("response_lid").Any())
            {
                parsedQuestion.Type = item.Element("response_lid").Attribute("rcardinality").Value == "Multiple"
                    ? ParsedQuestionType.MultipleChoice
                    : ParsedQuestionType.MultipleAnswer;

                parsedQuestion.Choices = ProccessChoiceAnswers(item);
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            if (item.Element("presentation").Elements("response_str").Any())
            {
                parsedQuestion.Type = ParsedQuestionType.ShortAnswer;
                parsedQuestion.Choices = ProccessShortAnswer(item);
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            parsedQuestion.Type = ParsedQuestionType.Essay;
            fileValidationResult.Questions.Add(parsedQuestion);
        }

        private IList<ParsedQuestionChoice> ProccessShortAnswer(XElement item)
        {
            throw new NotImplementedException();
        }

        private IList<ParsedQuestionChoice> ProccessChoiceAnswers(XElement item)
        {
            throw new NotImplementedException();
        }

        private int GetLineNumber(XElement item)
        {
            return ((IXmlLineInfo)item).HasLineInfo() ? ((IXmlLineInfo)item).LineNumber : -1;
        }

        private ParsedQuestion ParseMultiChoiceQuestion(XElement item)
        {
            var parsedQuestion = new ParsedQuestion();
          

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
            choice.IsCorrect = correctAnswers.Select(x => x.Attribute("title").Value).Any(x => x.Contains(choice.Text));

            return choice;
        }
    }
}
