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
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QTI
{
    public class QTIQuestionParser : QuestionParserBase
    {
      
        private FileValidationResult fileValidationResult;
        private const string MultiAnswerTypeName = "Multiple";
        private const string XmlNameSpacePattern = @"(xmlns:?[^=]*=[""][^""]*[""])";
        private const string AddAction = "Add";
        private const string SetAction = "Set";
        private const string ActivatedActionValue = "1";

        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QTI), StringComparison.CurrentCultureIgnoreCase);
        }

        public override ValidationResult Parse(string fileName, byte[] file)
        {
            PrepareNewResult(fileName);
            ProccessFile(file, fileName);
            Result.FileValidationResults.Add(fileValidationResult);
            return Result;
        }

        private void PrepareNewResult(string fileName)
        {
            fileValidationResult = new FileValidationResult
            {
                FileName = fileName,
                Questions = new List<ParsedQuestion>(),
                ValidationErrors = new List<string>()
            };
            Result = new ValidationResult
            {
                FileValidationResults = new List<FileValidationResult>()
            };
        }

        private string RemoveXmlNamespace(string xml)
        {
            return Regex.Replace(xml, XmlNameSpacePattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }


        private void ProccessFile(byte[] file, string fileName)
        {
            XDocument data;
            var itemsXml = new List<XElement>();
            try
            {
                data = XDocument.Parse(RemoveXmlNamespace(Encoding.UTF8.GetString(file)), LoadOptions.SetLineInfo);

                itemsXml = data.Descendants(XmlConsts.ItemName).ToList();
            }
            catch (Exception e)
            {
                Result.FileValidationResults.Add(new FileValidationResult
                {
                    FileName = fileName,
                    Questions = new List<ParsedQuestion>(),
                    ValidationErrors = new List<string>
                                       {
                                          String.Format("File: {0}, Line: 1 - Error during opening QTI Format", fileName)
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
                    fileValidationResult.ValidationErrors.Add(String.Format("File: {0}, Line: {1} - Error during question processing",fileName ,GetLineNumber(item)));
                    fileValidationResult.Questions.Add(new ParsedQuestion(){IsParsed = false});
                }

            }
        }


        private void ProcessXmlItem(XElement item)
        {
            var parsedQuestion = new ParsedQuestion();
            parsedQuestion.Title = item.Attribute(XmlConsts.TitleAttribute).Value;
            parsedQuestion.Text = item.Element(XmlConsts.PresentationName).Descendants().First().Value;
            var itemFeedBacks = item.Descendants(XmlConsts.FeedBackElementName);
            parsedQuestion.Feedback = !itemFeedBacks.Any(x => x.Attribute(XmlConsts.IdAttrName).Value == XmlConsts.GeneralId)
                ? string.Empty
                : itemFeedBacks.Single(x => x.Attribute(XmlConsts.IdAttrName).Value == XmlConsts.GeneralId).Value;

            parsedQuestion.MetadataSection = GetMetadata(item);
            parsedQuestion.IsParsed = true;
            if (item.Descendants(XmlConsts.ChoiceElementName).Any())
            {
                parsedQuestion.Type = item.Descendants(XmlConsts.ChoiceElementName).First().Attribute(XmlConsts.ChoiceTypeAttribute).Value == MultiAnswerTypeName
                    ? ParsedQuestionType.MultipleAnswer
                    : ParsedQuestionType.MultipleChoice;

                parsedQuestion.Choices = ProccessChoiceAnswers(item, itemFeedBacks);
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            if ((item.Element(XmlConsts.PresentationName).Elements(XmlConsts.FlowName).Any() && item.Descendants(XmlConsts.AnswerElementName).Any())||
                item.Descendants(XmlConsts.SolutionName).Any())
            {
              
                parsedQuestion.Choices = ProccessShortAnswer(item);
                parsedQuestion.Type = parsedQuestion.Choices.Any() ? ParsedQuestionType.ShortAnswer: ParsedQuestionType.Essay;
                fileValidationResult.Questions.Add(parsedQuestion);
                return;
            }

            parsedQuestion.Type = ParsedQuestionType.Essay;
            
            fileValidationResult.Questions.Add(parsedQuestion);
        }

        private SerializableDictionary<string, List<string>> GetMetadata(XElement item)
        {
            var result = new SerializableDictionary<string, List<string>>();
            var metadataFields = item.Descendants(XmlConsts.MetadataElementName);
            if (!metadataFields.Any())
            {
                return result;
            }

            foreach (var metadataField in metadataFields)
            {
                var name = metadataField.Element(XmlConsts.MetaFieldIdName).Value;
                var value = metadataField.Element(XmlConsts.MetaFieldValueName).Value;
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
            string text = null;
            try
            {
                var answerVarId = item.Descendants(XmlConsts.AnswerElementName).First().Attribute(XmlConsts.IdAttrName).Value;
                text = GetResponseVarByRespId(item.Descendants(XmlConsts.RepsonseVariableName), answerVarId).Descendants(XmlConsts.VarequalElementName).First().Value;
            }
            catch (Exception)
            {
              
            }

            if (!string.IsNullOrEmpty(text))
            {
                choices.Add(new ParsedQuestionChoice { IsCorrect = true, Text = text, Id = text });
                return choices;
            }

            try
            {
                text = item.Descendants(XmlConsts.SolutionName).First().Descendants(XmlConsts.MattextName).First().Value;
            }
            catch (Exception)
            {
                return choices;
            }

            choices.Add(new ParsedQuestionChoice { IsCorrect = true, Text = text, Id = text });
            return choices;
        }

    

        private List<ParsedQuestionChoice> ProccessChoiceAnswers(XElement item, IEnumerable<XElement> itemFeedBacks)
        {
            var choices = new List<ParsedQuestionChoice>();
            var responces = item.Descendants(XmlConsts.ResponseLabelName);
            if (!responces.Any())
            {
                return choices;
            }

            var respConditions = item.Descendants(XmlConsts.RepsonseVariableName);

            foreach (var responce in responces)
            {
                var choice = new ParsedQuestionChoice();
                var choiceIdXml = responce.Attribute(XmlConsts.IdAttrName);
                choice.Text = responce.Descendants(XmlConsts.MattextName).First().Value;
                choice.IsCorrect = IsCorrect(choiceIdXml, respConditions);
                choice.Feedback = GetFeedback(choiceIdXml, respConditions, itemFeedBacks);
                choice.Id = choiceIdXml == null ? choice.Text : choiceIdXml.Value;
                choices.Add(choice);
            }

            
            return choices;
        }

        private string GetFeedback(XAttribute choiceIdXml, IEnumerable<XElement> respConditions, IEnumerable<XElement> itemFeedBacks)
        {
            if (choiceIdXml == null || !respConditions.Any() || !itemFeedBacks.Any())
            {
                return string.Empty;
            }

            var responseVar = GetResponseVarByChoiceId(respConditions, choiceIdXml.Value);
            if (responseVar == null)
            {
                return string.Empty;
            }

            var feedBackId = responseVar.Element(XmlConsts.DisplayFeedbackElementName) == null
                ? string.Empty
                : responseVar.Element(XmlConsts.DisplayFeedbackElementName).Attribute(XmlConsts.LinkRefIdAttribute).Value;

            if (string.IsNullOrEmpty(feedBackId) || feedBackId == XmlConsts.GeneralId)
            {
                return string.Empty;
            }

            if (itemFeedBacks.Any(x => x.Attribute(XmlConsts.IdAttrName).Value == feedBackId))
            {
               return itemFeedBacks.First(x => x.Attribute(XmlConsts.IdAttrName).Value == feedBackId)
                                   .XPathSelectElement(XmlConsts.MattextXPath)
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

            XElement responseVar = GetResponseVarByChoiceId(respConditions, choiceIdXml.Value);
            if (responseVar == null)
            {
                return false;
            }

            return
                responseVar.Elements(XmlConsts.SetVarElementName)
                    .Any(x => x.Attribute(XmlConsts.ActionAttribute) != null &&
                              (x.Attribute(XmlConsts.ActionAttribute).Value == AddAction ||
                               x.Attribute(XmlConsts.ActionAttribute).Value == SetAction) &&
                              x.Value == ActivatedActionValue);
        }

        
        

        private XElement GetResponseVarByChoiceId(IEnumerable<XElement> respConditions, string id)
        {
            return
                respConditions.FirstOrDefault(
                    x =>
                        x.Element(XmlConsts.ConditionVarName) != null &&
                        x.XPathSelectElements(XmlConsts.VarequalXPath).Any(y => y.Value == id));
        }

        private XElement GetResponseVarByRespId(IEnumerable<XElement> respConditions, string id)
        {
            return
                respConditions.FirstOrDefault(
                    x =>
                        x.Element(XmlConsts.ConditionVarName) != null &&
                        x.XPathSelectElements(XmlConsts.VarequalXPath)
                            .Any(y => y.Attribute(XmlConsts.RespIdAttribute).Value == id));
        }


        private int GetLineNumber(XElement item)
        {
            return ((IXmlLineInfo)item).HasLineInfo() ? ((IXmlLineInfo)item).LineNumber : -1;
        }


    }
}
