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
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public List<string> QuestionValues { get; set; }

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
            QuestionValues = new List<string>();
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
            var element = new XElement("course");
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


                var listOfValues = element.Elements("values");
                if (listOfValues.Elements("value") != null)
                {
                    foreach (XElement questionValues in listOfValues.Elements("value"))
                    {
                        if (questionValues != null)
                        {
                            this.QuestionValues.Add(questionValues.Value);
                        }
                    }
                }
            }
        }

        #endregion
    }
}