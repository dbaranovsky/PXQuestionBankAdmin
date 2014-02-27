using System.Collections.Generic;
using System.Xml;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all configuration information to determine the behavior of
    /// any widget in the system
    /// </summary>
    public class WidgetConfiguration : ContentItem
    {
        /// <summary>
        /// a css class (or classes) that will be placed on the widget's wrapper
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }

        /// <summary>
        /// The name of the controller that implements the widget, without the
        /// Controller suffix. E.g. if the controller's full class name is FooWidgetController
        /// then this property should be set to FooWidget.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public string Controller { get; set; }

        /// <summary>
        /// The name of the action method to invoke on the controller
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; }

        /// <summary>
        /// Any additional parameters that need to be passed to the widgets Controller/Action
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Relative order in which to render this widget
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Whether or not this widget's view all link should load in Focused and Engaged mode
        /// </summary>
        /// <value>
        ///   <c>true</c> if [view all fne]; otherwise, <c>false</c>.
        /// </value>
        public bool ViewAllFne { get; set; }

        /// <summary>
        /// WidgetConfiguration
        /// </summary>
        /// <value>
        /// The type of the sub.
        /// </value>
        public string SubType { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is collapsed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollapsed { get; set; }

        public bool IsTitleHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is view all supported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is view all supported; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewAllSupported { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool ListStudents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the target id.
        /// </summary>
        /// <value>
        /// The target id.
        /// </value>
        public string TargetId { get; set; }

        /// <summary>
        /// Constructs a default WidgetConfiguration
        /// </summary>
        public WidgetConfiguration()
        {
            Parameters = new Dictionary<string, object>();
            Type = "WidgetConfiguration";
            TrackMinutesSpent = false;
            IsCollapsed = false;
            IsVisible = true;
            IsTitleHidden = false;
            ListStudents = true;
            // IsViewAllSupported = true;
        }
    }

    public class WidgetConfigurationCollection
    {
        /// <summary>
        /// List of possible sorts
        /// </summary>
        /// <value>
        /// The allowed widgets master list.
        /// </value>
        public XmlDocument AllowedWidgetsMasterList { get; set; }

        /// <summary>
        /// Gets or sets the current widget list.
        /// </summary>
        /// <value>
        /// The current widget list.
        /// </value>
        public string CurrentWidgetList { get; set; }

        /// <summary>
        /// All EPortfolio items
        /// </summary>
        public List<WidgetConfiguration> Widgets { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetConfigurationCollection"/> class.
        /// </summary>
        public WidgetConfigurationCollection()
        {
            Widgets = new List<WidgetConfiguration>();
            AllowedWidgetsMasterList = new XmlDocument();
            CurrentWidgetList = "";
        }
    }
}
