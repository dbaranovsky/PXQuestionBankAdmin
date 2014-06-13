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
        /// If fiels is filterable
        /// </summary>
        [DataMember]
        public bool Filterable { get; set; }

        /// <summary>
        /// If field should be displayed in in question banks
        /// </summary>
        [DataMember]
        public bool DisplayInBanks { get; set; }

        /// <summary>
        /// If field should be shown in filter in question banks
        /// </summary>
        [DataMember]
        public bool ShowFilterInBanks { get; set; }

        /// <summary>
        /// If field should be match in question banks
        /// </summary>
        [DataMember]
        public bool  MatchInBanks{ get; set; }

        /// <summary>
        /// If field should be displayed in in current quiz
        /// </summary>
        [DataMember]
        public bool DisplayInCurrentQuiz { get; set; }

        /// <summary>
        /// If field should be displayed in in instructor quiz
        /// </summary>
        [DataMember]
        public bool DisplayInInstructorQuiz { get; set; }

        /// <summary>
        /// If field should be displayed in in resources panel
        /// </summary>
        [DataMember]
        public bool DisplayInResources { get; set; }

        /// <summary>
        /// If field should be shown in filter in resources panel
        /// </summary>
        [DataMember]
        public bool ShowFilterInResources { get; set; }

        /// <summary>
        /// If field should be match in resources panel
        /// </summary>
        [DataMember]
        public bool MatchInResources { get; set; }

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
            element.Add(new XAttribute("displayinbanks", DisplayInBanks));
            element.Add(new XAttribute("showfilterinbanks", ShowFilterInBanks));
            element.Add(new XAttribute("matchinbanks", MatchInBanks));
            element.Add(new XAttribute("displayincurrentquiz", DisplayInCurrentQuiz));
            element.Add(new XAttribute("displayininstructorquiz", DisplayInInstructorQuiz));
            element.Add(new XAttribute("displayinresources", DisplayInResources));
            element.Add(new XAttribute("showfilterinresources", ShowFilterInResources));
            element.Add(new XAttribute("matchinresources", MatchInResources));
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                element.Add(new XAttribute("searchterm", SearchTerm));
            }
            if (!string.IsNullOrEmpty(FriendlyName))
            {
                element.Add(new XAttribute("friendlyname", FriendlyName));
            }
            if (!string.IsNullOrEmpty(Type))
            {
                element.Add(new XAttribute("type", Type));
            }
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

                if (element.Attribute("displayinbanks") != null)
                {
                    bool displayInBanks;
                    bool.TryParse(element.Attribute("displayinbanks").Value.ToString(), out displayInBanks);
                    this.DisplayInBanks = displayInBanks;
                }

                if (element.Attribute("showfilterinbanks") != null)
                {
                    bool showFilterInBanks;
                    bool.TryParse(element.Attribute("showfilterinbanks").Value.ToString(), out showFilterInBanks);
                    this.ShowFilterInBanks = showFilterInBanks;

                }

                if (element.Attribute("matchinbanks") != null)
                {
                    bool matchInBanks;
                    bool.TryParse(element.Attribute("matchinbanks").Value.ToString(), out matchInBanks);
                    this.MatchInBanks = matchInBanks;

                }

                if (element.Attribute("displayincurrentquiz") != null)
                {
                    bool displayInCurrentQuiz;
                    bool.TryParse(element.Attribute("displayincurrentquiz").Value.ToString(), out displayInCurrentQuiz);
                    this.DisplayInCurrentQuiz = displayInCurrentQuiz;

                }

                if (element.Attribute("displayininstructorquiz") != null)
                {
                    bool displayInInstructorQuiz;
                    bool.TryParse(element.Attribute("displayininstructorquiz").Value.ToString(), out displayInInstructorQuiz);
                    this.DisplayInInstructorQuiz = displayInInstructorQuiz;

                }

                if (element.Attribute("displayinresources") != null)
                {
                    bool displayInResources;
                    bool.TryParse(element.Attribute("displayinresources").Value.ToString(), out displayInResources);
                    this.DisplayInResources = displayInResources;

                }

                if (element.Attribute("showfilterinresources") != null)
                {
                    bool showFilterInResources;
                    bool.TryParse(element.Attribute("showfilterinresources").Value.ToString(), out showFilterInResources);
                    this.ShowFilterInResources = showFilterInResources;

                }

                if (element.Attribute("matchinresources") != null)
                {
                    bool matchInResources;
                    bool.TryParse(element.Attribute("matchinresources").Value.ToString(), out matchInResources);
                    this.MatchInResources = matchInResources;

                }

                if (element.Attribute("friendlyname") != null)
                {
                    this.FriendlyName = element.Attribute("friendlyname").Value.ToString();
                }

                if (element.Attribute("searchterm") != null)
                {
                    this.SearchTerm = element.Attribute("searchterm").Value.ToString();

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
}