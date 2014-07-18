using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            XDocument data;

            fileValidationResult = new FileValidationResult
                                   {
                                       FileName = fileName,
                                       Questions = new List<ParsedQuestion>(),
                                       ValidationErrors = new List<string>()
                                   };
            result = new ValidationResult
                     {
                         FileValidationResults = new List<FileValidationResult>()
                     };

            var itemsXml = new List<XElement>();
            try
            {
                data = XDocument.Parse(Regex.Replace(Encoding.UTF8.GetString(file),
                    @"(xmlns:?[^=]*=[""][^""]*[""])", "",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline));

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
                                                                                "Error during parsing QTI Format: {0}",
                                                                                e.Message)
                                                                        }
                                                 });
            }

            foreach (var item in itemsXml)
            {
                try
                {
                    ProcessXmlItem(item);
                }
                catch (Exception e)
                {
                    fileValidationResult.ValidationErrors.Add(String.Format("Line {0}: Question parse error: {1}", GetLineNumber(item), e.Message));
                }
               
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

            parsedQuestion.MetadataSection = GetMetadata(item);

            if (item.Descendants("response_lid").Any())
            {
                parsedQuestion.Type = item.Descendants("response_lid").First().Attribute("rcardinality").Value == "Multiple"
                    ? ParsedQuestionType.MultipleChoice
                    : ParsedQuestionType.MultipleAnswer;

                parsedQuestion.Choices = ProccessChoiceAnswers(item, itemFeedBacks);
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            if (item.Element("presentation").Elements("flow").Any() && item.Descendants("response_str").Any())
            {
              
                parsedQuestion.Choices = ProccessShortAnswer(item);
                parsedQuestion.Type = parsedQuestion.Choices.Any() ? ParsedQuestionType.ShortAnswer: ParsedQuestionType.Essay;
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            parsedQuestion.Type = ParsedQuestionType.Essay;
            fileValidationResult.Questions.Add(parsedQuestion);
        }

        private Dictionary<string, List<string>> GetMetadata(XElement item)
        {
            var result = new Dictionary<string, List<string>>();
            var metadataFields = item.Descendants("qtimetadatafield");
            if (!metadataFields.Any())
            {
                return result;
            }

            foreach (var metadataField in metadataFields)
            {
                var name = metadataField.Element("fieldlabel").Value;
                var value = metadataField.Element("fieldentry").Value;
                if (!result.ContainsKey(name))
                {
                    result.Add(name, new List<string>{value});
                }
                else
                {
                    result[name].Add(value);
                }
            }

            return result;
        }

        private List<ParsedQuestionChoice> ProccessShortAnswer(XElement item)
        {
            var choices = new List<ParsedQuestionChoice>();
            string text;
            try
            {
                var answerVarId = item.Descendants("response_str").First().Attribute("ident").Value;
                text = GetResponseVar(item.Descendants("respcondition"), answerVarId).Descendants("mattext").First().Value;
  
            }
            catch (Exception e)
            {
                return choices;
            }

 
           var shortAnswer = new ParsedQuestionChoice();
            shortAnswer.IsCorrect = true;
            shortAnswer.Text = text;

           return choices;
        }

        private List<ParsedQuestionChoice> ProccessChoiceAnswers(XElement item, IEnumerable<XElement> itemFeedBacks)
        {
            var choices = new List<ParsedQuestionChoice>();
            var responces = item.Descendants("response_label");
            if (!responces.Any())
            {
                return choices;
            }

            var respConditions = item.Descendants("respcondition");

            foreach (var responce in responces)
            {
                var choice = new ParsedQuestionChoice();
                var choiceIdXml = responce.Attribute("ident");
                choice.Text = responce.Descendants("mattext").First().Value;
                choice.IsCorrect = IsCorrect(choiceIdXml, respConditions);
                choice.Feedback = GetFeedback(choiceIdXml, respConditions, itemFeedBacks);
            }

            
            return choices;
        }

        private string GetFeedback(XAttribute choiceIdXml, IEnumerable<XElement> respConditions, IEnumerable<XElement> itemFeedBacks)
        {
            if (choiceIdXml == null || !respConditions.Any() || !itemFeedBacks.Any())
            {
                return string.Empty;
            }

            var responseVar = GetResponseVar(respConditions, choiceIdXml.Value);
            if (responseVar == null)
            {
                return string.Empty;
            }

            var feedBackId = responseVar.Element("displayfeedback") == null
                ? string.Empty
                : responseVar.Element("displayfeedback").Attribute("linkrefid").Value;

            if (string.IsNullOrEmpty(feedBackId) || feedBackId == "general")
            {
                return string.Empty;
            }

            if (itemFeedBacks.Any(x => x.Attribute("ident").Value == feedBackId))
            {
               return itemFeedBacks.First(x => x.Attribute("ident").Value == feedBackId)
                                   .XPathSelectElement("material/mattext")
                                   .Value;
            }
            return string.Empty;
        }

        private bool IsCorrect(XAttribute choiceIdXml, IEnumerable<XElement> respConditions)
        {
            if (choiceIdXml == null || !respConditions.Any())
            {
                return false;
            }

            var responseVar = GetResponseVar(respConditions, choiceIdXml.Value);
            if (responseVar == null)
            {
                return false;
            }

            return responseVar.Elements("setvar").Any( x => x.Attribute("action") != null &&
                                                           (x.Attribute("action").Value == "Add" || x.Attribute("action").Value == "Set") &&
                                                            x.Value == "1");
        }

        private XElement GetResponseVar(IEnumerable<XElement> respConditions, string id)
        {
            return respConditions.FirstOrDefault(x => x.Element("conditionvar")!= null && x.Element("conditionvar").Elements("varequal").Any(y => y.Value == id));
        }

        private int GetLineNumber(XElement item)
        {
            return ((IXmlLineInfo)item).HasLineInfo() ? ((IXmlLineInfo)item).LineNumber : -1;
        }


    }
}
