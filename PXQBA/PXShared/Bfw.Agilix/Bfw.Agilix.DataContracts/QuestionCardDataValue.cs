using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Possible value of the question data metafield
    /// </summary>
    [DataContract]
    public class QuestionCardDataValue:IDlapEntityParser
    {
        /// <summary>
        /// Display text of the value
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Sequence of the value
        /// </summary>
        [DataMember]
        public int Sequence { get; set; }

        public XElement ToEntity(int i)
        {
            var element = new XElement("value");
            if (!string.IsNullOrEmpty(Text))
            {
                element.Add(new XAttribute("text", Text));
            }
            element.Add(new XAttribute("sequence", Sequence));
            return element;
        }

        public void ParseEntity(XElement element)
        {
            int outvalue;
            if (element.Attribute("text") != null)
            {
                Text = element.Attribute("text").Value;
            }
            if (element.Attribute("sequence") != null)
            {
                int.TryParse(element.Attribute("sequence").Value, out outvalue);
                Sequence = outvalue;
            }
        }
    }
}