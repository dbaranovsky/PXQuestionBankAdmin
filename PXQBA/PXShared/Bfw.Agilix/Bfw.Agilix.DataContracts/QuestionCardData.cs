using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Provides accessors on top of the basic Item contract
    /// </summary>
    /// 
    [DataContract]
    public class QuestionCardData : IDlapEntityParser
    {
        #region Properties

        
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string QuestionCardDataName { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public bool Filterable { get; set; }
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string SearchTerm { get; protected set; }

        /// <summary>
        /// Field type
        /// </summary>
        [DataMember]
        public string Type { get; protected set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public List<QuestionCardDataValue> QuestionValues { get; set; }

        /// <summary>
        /// Shows if meta field should be hidden
        /// </summary>
        [DataMember]
        public bool Hidden { get; set; }

        private string _dataString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }
        }

        [NonSerialized]
        private XElement _data;

        /// <summary>
        /// XML item data read from agilix item retrieval.
        /// </summary>
        /// 
        [DataMember]
        public XElement Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Question Card Constructor
        /// </summary>
        public QuestionCardData()
        {
            Data = new XElement("data");
            QuestionValues = new List<QuestionCardDataValue>();
        }

        #endregion

        #region Methods


        #endregion

        #region overriden from Item

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public XElement ToEntity()
        {
            var element = new XElement(QuestionCardDataName);
            element.Add(new XAttribute("filterable", Filterable));
            element.Add(new XAttribute("hidden", Hidden));
            element.Add(new XAttribute("searchterm", SearchTerm));
            element.Add(new XAttribute("friendlyname", FriendlyName));
            element.Add(new XAttribute("type", Type));
            for(var i=0; i<QuestionValues.Count; i++)
            {
                element.Add(QuestionValues[i].ToEntity(i));
            }
            return element;
        }

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(XElement element)
        {
            if (element != null)
            {
                QuestionCardDataName = element.Name.LocalName;
                bool outvalue = false;
                if (element.Attribute("filterable") != null)
                {
                    bool.TryParse(element.Attribute("filterable").Value.ToString(), out outvalue);
                    this.Filterable = outvalue;

                }

                if (element.Attribute("friendlyname") != null)
                {

                    this.FriendlyName = element.Attribute("friendlyname").Value.ToString();
                }

                if (element.Attribute("searchterm") != null)
                {
                    this.SearchTerm = element.Attribute("searchterm").Value.ToString();

                }

                if (element.Attribute("hidden") != null)
                {
                    bool.TryParse(element.Attribute("hidden").Value.ToString(), out outvalue);
                    this.Hidden = outvalue;

                }

                if (element.Attribute("type") != null)
                {
                    this.Type = element.Attribute("type").Value;
                }

                QuestionValues = new List<QuestionCardDataValue>();
                var listOfValues = element.Elements("value");
                if (listOfValues.Any())
                {
                    foreach (XElement questionValueElement in listOfValues)
                    {
                        var questionValue = new QuestionCardDataValue();
                        if (questionValueElement != null)
                        {
                            questionValue.ParseEntity(questionValueElement);
                            QuestionValues.Add(questionValue);
                        }
                    }
                }
            }
        }

        #endregion
    }

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
            element.Add(new XAttribute("text", Text));
            element.Add(new XAttribute("sequence", i));
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