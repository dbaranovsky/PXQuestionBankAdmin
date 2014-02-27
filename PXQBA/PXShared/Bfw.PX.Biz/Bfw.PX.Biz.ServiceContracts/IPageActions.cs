
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Bfw.PX.Biz.DataContracts;
using System.Collections;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// provide functionality to load all zone and widegts for the page.
    /// </summary>
    public interface IPageActions
    {
        /// <summary>
        /// Loads the Page item along with all of its Zones, and Widgets in those Zones.   
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        PageDefinition LoadPageDefinition(string pageName);

        /// <summary>
        /// Loads the menu and menu items and templates.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        Menu LoadMenu(string menuId);

        /// <summary>
        /// Moves the Widget so that it is in the specified zone at the specified postion.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneName"></param>
        /// <param name="widgetId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        string MoveWidget(string pageName, string zoneName, string widgetId, string minSequence, string maxSequence);

        /// <summary>
        /// Changes the widget's 'bfw_display_flags' values to match the WidgetDisplayOptions specified. 
        /// This can be used to hide the widget from view in matching cases.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="widgetDisplayOptions"></param>
        void SetWidgetDisplay(string widgetId, WidgetDisplayOptions widgetDisplayOptions);

        /// <summary>
        /// Removes the Widget from any Zone it is currently in.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="pageName"></param>
        void RemoveWidget(string widgetId, string pageName);

        /// <summary>
        /// This method will create a copy of the specified widget and puts it in the correct page zone. 
        /// The newly created widget will be returned.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="zoneId"></param>
        /// <param name="widgetTemplateId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <returns></returns>
        Widget AddWidget(string pageName, string zoneId, string widgetTemplateId, string minSequence, string maxSequence, string newWidgetId);

        /// <summary>
        /// This method will create a new copy of the item with templateId. Any properties specified in the properties collection
        /// will be override the values in the template.  The resulting widget will be created and saved.
        /// </summary>
        /// <param name="pageName">Name of the page to add the widget to, e.g. "Home".</param>
        /// <param name="zoneId">ID of the zone to add the widget to.</param>
        /// <param name="templateId">ID of the template</param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <param name="newWidget"></param>
        /// <returns></returns>
        Widget AddWidget(string pageName, string zoneId, string templateId, string minSequence, string maxSequence, string title, IDictionary<string, PropertyValue> properties);

        /// <summary>
        /// This method will update an exiting widget by using the given property values.
        /// </summary>
        /// <param name="pageName">name of the page the widget is on.</param>
        /// <param name="widgetId">id of the widget being edited</param>
        /// <param name="title">New title of the widget. If null or empty is passed then the title is not changed.</param>
        /// <param name="properties">custom properties being changed.</param>
        /// <returns></returns>
        Widget UpdateWidget(string pageName, string widgetId, string title, IDictionary<string, PropertyValue> properties);

        /// <summary>
        /// This method will empty the cache for the given widget's page-definition
        /// </summary>
        /// <param name="id">id of the widget</param>
        /// <returns></returns>
        void EmptySettingsCache(string id);

        /// <summary>
        /// This method will update an exiting widget using the widget object
        /// </summary>
        /// <returns></returns>
        void UpdateWidget(Widget w);
        
        /// <summary>
        /// This method will get a specified widget.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="widgetTemplateId"></param>
        /// <returns></returns>
        Widget GetWidgetTemplate(string widgetTemplateId);


        /// <summary>
        /// Get the widget bases on widget id
        /// </summary>
        /// <param name="widgetId">id of the widget that has to be found</param>
        /// <returns></returns>
        Widget GetWidget(string widgetId);


        /// <summary>
        /// Get the widget based on widget id
        /// </summary>
        /// <param name="widgetId">id of the widget that has to be found</param>
        /// <returns></returns>
        InstructorConsoleSettings GetInstructorConsoleSettings(string widgetId);

        /// <summary>
        /// Get the widget based on widget id
        /// </summary>
        /// <returns></returns>
        LaunchPadSettings GetInstructorConsoleLaunchPadSettings();

        /// <summary>
        /// This method will create a copy of the specified Menu Item from the a template. 
        /// The newly created widget will be returned.
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="menuItemId"></param>
        /// <param name="templateId"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        //MenuItem AddMenuItem(string menuId, string menuItemId, string templateId, string title, string url, string minSequence, string maxSequence, IDictionary parameters);

        MenuItem AddMenuItem(string menuId, MenuItem menuItem, IDictionary parameters);

        /// <summary>
        /// Moves the Menu Item so that it is in the specified zone at the specified postion.
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <param name="minSequence"></param>
        /// <param name="maxSequence"></param>
        /// <param name="menuId"></param>
        string MoveMenuItem(string menuId, string menuItemId, string minSequence, string maxSequence);

        /// <summary>
        /// Removes the Menu Item.
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="menuId"></param>
        void RemoveMenuItem(string menuId, string menuItemId);

        /// <summary>
        /// Rename the Course.
        /// </summary>
        /// <param name="courseName"></param>
        void RenameCourse(string courseName, string pageName);

    }
}

