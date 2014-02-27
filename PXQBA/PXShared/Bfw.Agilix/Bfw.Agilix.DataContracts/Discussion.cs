using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a discussion between users.
    /// </summary>
    public class Discussion : IDlapEntityParser, IDlapEntityTransformer, IItem
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
        /// Id of the item that acts as the resource folder (typically the same as Id)
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        /// 
        [DataMember]
        public Item Item { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Discussion()
        {
            Item = new Item();
        }

        #endregion

        #region IDlapEntityParser Members

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

        /// <summary>
        /// Parses extra assignment information from the root element.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
        protected void ParseExtraData(XElement data)
        {
            if (null != data)
            {
                var urlElm = data.Element("href");
                var period = data.Element("period");
                var gradable = data.Element("gradable");
                var dueDate = data.Element("duedate");
                var folder = data.Element("folder");

                if (null != urlElm)
                    Url = urlElm.Value;

                if (null != folder)
                    Folder = folder.Value;

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
            }
        }

        #endregion

        #region IDlapEntityTransformer Members

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
            }

            return item;
        }

        #endregion

        #region IItem Members

        /// <summary>
        /// Implementor can turn its internal state into an Agilix Item
        /// </summary>
        /// <returns><see cref="Discussion" /> object represented as an Item.</returns>
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
