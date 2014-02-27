using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace Bfw.PX.XBkPlayer.Biz.DataContracts
{
    [DataContract]
    public class XBkPlayerData
    {
        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string EnrollmentId { get; set; }

        [DataMember]
        public string QuestionId { get; set; }

        [DataMember]
        public List<XBkQuestion> Questions { get; set; }

        [DataMember]
        public string SaveUrl { get; set; }

        [DataMember]
        public string QuestionBody { get; set; }

        public void LoadQuestion(string questionXML)
        {
            var xPathDoc = new XPathDocument(questionXML);
            var xPathNav = xPathDoc.CreateNavigator();

            ParseQuestions(xPathNav);
        }

        public void ParseQuestions(XPathNavigator nodeNav)
        {
            XPathNodeIterator nodes = nodeNav.Select("/info/question");
            if (nodes != null)
            {
                Questions = new List<XBkQuestion>();

                while (nodes.MoveNext())
                {
                    var question = ToQuestion(nodes.Current);
                    var type = "";
                    question.InteractionData = GetInteraction(nodes.Current, out type);
                    if (type == "text" || type == "essay")
                    {
                        question.Type = type;
                        //question.SearchableMetaData = GetMetaData(nodes.Current);
                        question.Answer = GetAnswer(nodes.Current);
                        question.Text = GetQuestionText(nodes.Current);
                        Questions.Add(question);
                    }
                }
            }
        }

        public string GetQuestionText(XPathNavigator questionNav)
        {
            XPathNodeIterator nodesText = questionNav.Select("./body");
            nodesText.MoveNext();

            string questionTxt = "";
            if (nodesText != null)
            {
                questionTxt = nodesText.Current.Value.ToString();
            }

            return questionTxt;
        }

        public XBkQuestion ToQuestion(XPathNavigator questionNav)
        {
            var entityId = questionNav.GetAttribute("resourceentityid", "").Split(',').FirstOrDefault().ToString();
            var question = new XBkQuestion { Id = questionNav.GetAttribute("questionid", ""), EntityId = entityId };
            return question;
        }

        public Dictionary<string, string> GetMetaData(XPathNavigator nodeNav)
        {
            XPathNodeIterator nodesText = nodeNav.Select("./meta");
            nodesText.MoveNext();
            Dictionary<string, string> oMeta = null;
            if (nodesText.Current.HasChildren)
            {
                oMeta = new Dictionary<string, string>();

                nodesText.Current.MoveToFirstChild();

                do
                {
                    oMeta.Add(nodesText.Current.Name, nodesText.Current.Value.ToString());
                } while (nodesText.Current.MoveToNext());
            }

            return oMeta;
        }

        public string GetAnswer(XPathNavigator nodeNav)
        {
            XPathNodeIterator nodesText = nodeNav.Select("./answer");
            string answer = "";
            nodesText.MoveNext();
            if (nodesText.Current.HasChildren)
            {
                nodesText.Current.MoveToFirstChild();
                answer = nodesText.Current.Value.ToString();
            }

            return answer;
        }

        public string GetInteraction(XPathNavigator nodeNav, out string type)
        {
            XPathNodeIterator nodesText = nodeNav.Select("./interaction");
            var interaction = "";
            nodesText.MoveNext();
            type = nodesText.Current.GetAttribute("type", "").ToString();
            if (type == "essay" || type == "text")
            {
                interaction = nodesText.Current.Value;
            }

            return interaction;
        }
    }
}
