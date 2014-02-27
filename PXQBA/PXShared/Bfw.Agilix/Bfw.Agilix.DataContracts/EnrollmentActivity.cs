using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Information about the recent activity of an enrollment.
    /// </summary>
    class EnrollmentActivity : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Data Members

        /// <summary>
        /// Id of the last accessed item.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Date and time item was last accessed.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Duration the item was viewed.
        /// </summary>
        public string Seconds { get; set; }

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
            if (null != element)
            {
                var itemidAttr = element.Attribute("itemid");
                var dateAttr = element.Attribute("date");
                var secondsAttr = element.Attribute("seconds");

                if (null != itemidAttr)
                    ItemId = itemidAttr.Value;

                if (null != dateAttr)
                {
                    DateTime dt;
                    if (DateTime.TryParse(dateAttr.Value, out dt))
                        Date = dt;
                }

                if (null != secondsAttr)
                    Seconds = secondsAttr.Value;
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
            var element = new XElement("enrollment",
                new XAttribute("itemid", ItemId),
                new XAttribute("date", DateRule.Format(Date)),
                new XAttribute("seconds", Seconds));

            return element;
        }

        #endregion
    }
}
