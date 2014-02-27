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
    /// Represents a message broadcast to either the domain as a whole or a specific course/section
    /// </summary>
    /// 
    [DataContract]
    public class Announcement : IDlapEntityParser, IDlapEntityTransformer
    {
        #region Data Members

        /// <summary>
        /// The entity that owns the announcement
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Path to the zip file containing the announcement data
        /// </summary>
        [DataMember]
        public string Path { get; set; }

        /// <summary>
        /// Date after which the announcement should display
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date before which the announcement should display
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Date the announcement was created
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Title of the announcement
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Body of the announcement in HTML form
        /// </summary>
        [DataMember]
        public string Html { get; set; }

        /// <summary>
        /// Version of the announcement
        /// </summary>
        [DataMember]
        public String Version { get; set; }

        /// <summary>
        /// sequence id of an announcement
        /// </summary>
        [DataMember]
        public string PrimarySortOrder { get; set; }

        /// <summary>
        /// Pin position of an announcement
        /// </summary>
        [DataMember]
        public string PinSortOrder { get; set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Set this announcement's properties by inspecting an XElement
        /// </summary>
        /// <param name="element">Root element of the announcement metadata</param>
        public void ParseEntity(XElement element)
        {
            if (null != element)
            {
                if ("announcement" != element.Name)
                    throw new DlapEntityFormatException(string.Format("Announcement entity expects an element name of announcement, but got {0}", element.Name));

                var eid = element.Attribute("entityid");
                var p = element.Attribute("path");
                var cdt = element.Attribute("creationdate") ?? element.Attribute("created");
                var sdt = element.Attribute("startdate");
                var edt = element.Attribute("enddate");
                var ti = element.Attribute("title");
                var body = element.Element("body");
                var primarySortOrder = element.Element("PrimarySortOrder");
                var pinSortOrder = element.Element("PinSortOrder");

                if (null != eid)
                {
                    EntityId = eid.Value;
                }

                if (null != p)
                {
                    Path = p.Value;
                }

                if (null != cdt)
                {
                    DateTime dto;
                    if (DateTime.TryParse(cdt.Value, out dto))
                        CreationDate = dto;
                }

                if (null != sdt)
                {
                    DateTime dto;
                    if (DateTime.TryParse(sdt.Value, out dto))
                        StartDate = dto;
                }

                if (null != edt)
                {
                    DateTime dto;
                    if (DateTime.TryParse(edt.Value, out dto))
                        EndDate = dto;
                }

                if (null != ti)
                {
                    Title = ti.Value;
                }

                if (null != body)
                {
                    Html = body.Value;
                }

                if (null != primarySortOrder)
                {
                    PrimarySortOrder = primarySortOrder.Value;
                }

                if (null != pinSortOrder)
                {
                    PinSortOrder = pinSortOrder.Value;
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
            var element = new XElement("announcement",
                new XAttribute("entityid", EntityId),
                new XAttribute("title", Title),
                new XAttribute("startdate", StartDate),
                new XAttribute("enddate", EndDate),
                new XAttribute("recurse", "false"),
                new XAttribute("creationdate", CreationDate));

            if (Html != null)
            {
                var htmlBody = new XElement("body", Html.ToString());
                element.Add(htmlBody);
            }

            if (PrimarySortOrder != null)
            {
                var primarySortOrder = new XElement("PrimarySortOrder", PrimarySortOrder.ToString());
                element.Add(primarySortOrder);
            }

            if (PinSortOrder != null)
            {
                var pinSortOrder = new XElement("PinSortOrder", PinSortOrder.ToString());
                element.Add(pinSortOrder);
            }
            

            return element;
        }

        #endregion
    }
}
