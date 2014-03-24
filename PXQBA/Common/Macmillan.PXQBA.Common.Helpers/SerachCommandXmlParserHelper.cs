using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Common.Helpers
{
    /// <summary>
    /// Helper for parsing SerachCommand result
    /// </summary>
    public static class SerachCommandXmlParserHelper
    {
        /// <summary>
        /// Parse result document and append parsed questions in questionList
        /// </summary>
        /// <param name="document">XDocument from search command</param>
        /// <param name="questionList">Container for result</param>
        /// <param name="questionTypesDictionary">Available question types</param>
        /// <returns>Result of parsing </returns>
        public static QuestionList PareseResultXDocument(XDocument document, QuestionList questionList, Dictionary<string, string> questionTypesDictionary)
        {
            var result = document.Element("results").Element("result");
            string numFound = result.Attribute("numFound").Value;
            questionList.AllQuestionsAmount = Int32.Parse(numFound);
            
            var nodes = result.Elements("doc");

            foreach (var node in nodes)
            {
                string dlap_q_type = ExtractNode(node, "str", "name", "dlap_q_type");
                string dlap_id = ExtractNode(node, "str", "name", "dlap_id");
                string dlap_title = ExtractNode(node, "str", "name", "dlap_title");
                string dlap_html_text = ExtractNode(node, "str", "name", "dlap_html_text", "|");
                string question_id = node.Attribute("questionid").Value;

                var questionMetadata = new QuestionMetadata();

                questionMetadata.Data.Add("questionHtmlInlinePreview", dlap_html_text);
                questionMetadata.Data.Add("dlap_q_type", GetQuestionFullType(dlap_q_type, questionTypesDictionary));
                questionMetadata.Data.Add("dlap_title", question_id);
                questionMetadata.Data.Add("id", question_id);
                questionMetadata.Data.Add("chapter", "");
                questionMetadata.Data.Add("bank", "");
                questionMetadata.Data.Add("seq", "");

                questionList.Questions.Add(questionMetadata);
            }

            return questionList;
        }

        private static string GetQuestionFullType(string shortType, Dictionary<string, string> questionTypesDictionary)
        {
            if (questionTypesDictionary.ContainsKey(shortType))
            {
                return questionTypesDictionary[shortType];
            }

            return shortType;
        }


        /// <summary>
        /// LINQ based helper method to search and extract XML node data.
        /// </summary>
        /// <param name="xmlElement">The XML element.</param>
        /// <param name="nodeName">Name of the XML node.</param>
        /// <param name="nodeAttribute">Name of the XML attribute.</param>
        /// <param name="nodeValue">XML node value.</param>
        /// <param name="joinVal">String to use when joining values in an array</param>
        /// <returns></returns>
        public static string ExtractNode(XElement xmlElement, string nodeName, string nodeAttribute, string nodeValue, string joinVal = "")
        {
            string strRetVal = "";
            try
            {
                var elems = xmlElement.Elements().Where(a => a.Attribute(nodeAttribute).Value == nodeValue);
                var childElems = elems.Elements();
                if (childElems.Any())
                {
                    strRetVal += string.Join(joinVal, childElems.Select(a => a.Value));
                }
                else
                {
                    strRetVal = string.Join(joinVal, elems.Select(a => a.Value));
                }
            }
            catch { }
            return strRetVal;
        }
    }
}
