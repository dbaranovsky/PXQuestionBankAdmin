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
    public class Assignment : IDlapEntityParser, IDlapEntityTransformer, IItem
    {
        #region Properties

        /// <summary>
        /// Location of the assignment
        /// </summary>
        /// 
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// True if the assignment is gradeable
        /// </summary>
        /// 
        [DataMember]
        public bool Gradable { get; set; }

        /// <summary>
        /// Date and time the assignment is due
        /// </summary>
        /// 
        [DataMember]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Period of the assignment
        /// </summary>
        /// 
        [DataMember]
        public int Period { get; set; }

        /// <summary>
        /// True if the assignemnt accepts uploaded documents
        /// </summary>
        /// 
        [DataMember]
        public bool HasDropbox { get; set; }

        /// <summary>
        /// Determines configuration and layout of the dropbox
        /// </summary>
        /// 
        [DataMember]
        public DlapDropboxType DropboxType { get; set; }

        /// <summary>
        /// The grading category of the item
        /// </summary>
        /// 
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// String that identifies what determines when the assignment is considered
        /// completed.
        /// </summary>
        [DataMember]
        public CompletionTrigger CompletionTrigger { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        /// 
        [DataMember]
        public Item Item { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Assignment()
            : this(new Item())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> class.
        /// </summary>
        /// <param name="item">The full item this assignment represents.</param>
        /// <remarks></remarks>
        public Assignment(Item item)
        {
            Item = item;

            if (null != Item && null != Item.Data)
            {
                ParseExtraData(Item.Data);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the extra, assignment specific data from the document.
        /// </summary>
        /// <param name="data">The root XML item.</param>
        /// <remarks></remarks>
        protected void ParseExtraData(XElement data)
        {
            if (null != data)
            {
                var urlElm = data.Element("href");
                var hasBox = data.Element("dropbox");
                var boxType = data.Element("dropboxtype");
                var period = data.Element("period");
                var gradable = data.Element("gradable");
                var dueDate = data.Element("duedate");
                var category = data.Element("category");
                var completiontrigger = data.Element("completiontrigger");

                if (null != urlElm)
                    Url = urlElm.Value;

                if (null != hasBox)
                {
                    bool hb = false;
                    if (bool.TryParse(hasBox.Value, out hb))
                        HasDropbox = hb;
                }

                if (null != boxType)
                {
                    int bt = 0;
                    if (int.TryParse(boxType.Value, out bt))
                    {
                        if (Enum.IsDefined(typeof(DlapDropboxType), bt))
                        {
                            DropboxType = (DlapDropboxType)Enum.Parse(typeof(DlapDropboxType), boxType.Value);
                        }
                    }
                }

                if (null != period)
                {
                    int p = 0;
                    if (int.TryParse(period.Value, out p))
                    {
                        Period = p;
                    }
                }

                if (null != gradable)
                {
                    bool g = false;
                    if (bool.TryParse(gradable.Value, out g))
                        Gradable = g;
                }

                if (null != dueDate)
                {
                    DateTime dd;
                    if (DateTime.TryParse(dueDate.Value, out dd))
                    {
                        DueDate = dd;
                    }
                    else
                    {
                        DueDate = null;
                    }
                }
                else
                {
                    DueDate = null;
                }

                if (null != category)
                {
                    int cat = 0;
                    if (int.TryParse(category.Value, out cat))
                    {
                        Category = cat.ToString();
                    }
                }

                if (null != completiontrigger)
                {
                    int cat = 1;
                    if (int.TryParse(completiontrigger.Value, out cat))
                    {
                        CompletionTrigger = (CompletionTrigger) cat;
                    }
                }
            }
        }

        #endregion

        #region overriden from Item

        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        /// <remarks></remarks>
        public XElement ToEntity()
        {
            var item = Item.ToEntity();
            var data = item.Element("data");

            if (null != data)
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    var urlElm = data.Element("href");
                    if (null == urlElm)
                    {
                        urlElm = new XElement("href");
                        data.Add(urlElm);
                    }

                    urlElm.SetValue(Url);
                }

                var hasBox = data.Element("dropbox");
                var boxType = data.Element("dropboxtype");

                if (null == hasBox)
                    hasBox = new XElement("dropbox");

                if (HasDropbox)
                {
                    hasBox.SetValue(bool.TrueString);

                    if (null == boxType)
                    {
                        boxType = new XElement("dropboxtype");
                        data.Add(boxType);
                    }

                    boxType.SetValue((long)DropboxType);
                }
                else
                {
                    hasBox.SetValue(bool.FalseString);

                    if (null != boxType)
                        boxType.Remove();
                }

                var period = data.Element("period");

                if (null == period)
                {
                    period = new XElement("period");
                    data.Add(period);
                }

                period.SetValue(Period);

                var gradable = data.Element("gradable");

                if (null == gradable)
                {
                    gradable = new XElement("gradable");
                    data.Add(gradable);
                }

                gradable.SetValue(Gradable);

                var dueDate = data.Element("duedate");

                if (DueDate.HasValue)
                {
                    if (null == dueDate)
                    {
                        dueDate = new XElement("duedate");
                        data.Add(dueDate);
                    }

                    if (!DateRule.Validate(DueDate.Value))
                        throw new DlapException("DueDate must be between DateRule.MinDate and DateRule.MaxDate");

                    dueDate.SetValue(DueDate.Value);
                }
                else
                {
                    if (null != dueDate)
                        dueDate.Remove();
                }

                var category = data.Element("category");
                if (null == category)
                {
                    category = new XElement("duedate");
                    data.Add(category);
                }
                category.SetValue(int.Parse(Category));
            }

            return item;
        }

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(XElement element)
        {
            Item = new Item();
            Item.ParseEntity(element);

            var data = Item.Data;
            ParseExtraData(data);
        }

        #endregion

        #region IItem members

        /// <summary>
        /// Implementor can turn its internal state into an Agilix Item
        /// </summary>
        /// <returns>Item representation of an <see cref="Assignment" /></returns>
        /// <remarks></remarks>
        public Item AsItem()
        {
            var itemElm = ToEntity();
            var item = new Item();
            item.ParseEntity(itemElm);

            return item;
        }

        #endregion
    }
}