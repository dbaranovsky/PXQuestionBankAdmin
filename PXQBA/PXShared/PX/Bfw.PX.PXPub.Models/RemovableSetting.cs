using System;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// This is a setting particularly for TreeWidget.
    /// This determines whether to show or hide the "REMOVE" link on ManagementCard.
    ///     <bfw_property name="bfw_removable_switch" type="Boolean">true</bfw_property>
    ///		<bfw_property name="bfw_removable_xpath_query" type="String">//bfw_tocs[my_materials='my_materials']</bfw_property>
    ///		<bfw_property name="bfw_remove_from_toc" type="String">syllabusfilter,assignmentfilter</bfw_property>
    /// </summary>
    [Serializable]
    public class RemovableSetting
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show or hide "REMOVE" links on Management Cards of CONTENT TREE WIDGET ITEMS
        /// Turning on and off of this attribute affects all content items in this widget
        /// Below is an example of xml element in DLAP
        /// <bfw_removable switch="false"></bfw_removable> 
        /// </summary>
        /// <value>
        ///   <c>true</c> if all items are removable; otherwise, <c>false</c>.
        /// </value>
        public bool Switch { get; set; }

        /// <summary>
        /// Gets or sets the filter which is an XPath query.
        /// Using the XPath query, this filters the items in this widget that are removable.
        /// </summary>
        /// <value>
        /// The xpath query.
        /// </value>
        public String XPathQueryFilter { get; set; }

        /// <summary>
        /// Gets or sets the tocs/filters (e.g. syllabusfilter) from which the selected item will be removed.
        /// When an item is selected to be removed, removal flags are added to this list of TOCs.
        /// </summary>
        /// <value>
        /// The list of toc filters.
        /// </value>
        public String RemoveFromTocs { get; set; }
    }
}
