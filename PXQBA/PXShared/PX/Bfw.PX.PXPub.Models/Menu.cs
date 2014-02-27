using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;
using System.Web.Routing;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Runtime.Serialization;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    [DataContract]
    public class Menu
    {
        /// The unique Id of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// The title of the item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Abbreviation of the item.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// The bfw type of the item.
        /// </summary>
        public string BfwType { get; set; }

        /// <summary>
        /// The bfw sub type of the item.
        /// </summary>
        public string BfwSubType { get; set; }

        /// <summary>
        /// This list has values from the bfw_display_flags child elements
        /// </summary>
        /// 
        [DataMember (Name= "MenuItems")]
        public List<MenuItem> MenuItems { get; set; }

        /// <summary>
        /// The template items
        /// </summary>
        public List<MenuItem> MenuItemTemplates { get; set; }

        /// <summary>
        /// Count of all items and children items in menu
        /// </summary>
        public int FlatCount { get; set; }

        /// <summary>
        /// If true show tab index navigation, false otherwise.
        /// </summary>
        public bool IncludeIndexNavigation { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>

        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Place holder for the selected menu item
        /// </summary>
        public MenuItem SelectedMenuItem { get; set; }

        /// <summary>
        /// The TocId of the parent course
        /// </summary>
        public string BfwTocId { get; set; }

        public Menu()
        {
            MenuItems = new List<MenuItem>();
            MenuItemTemplates = new List<MenuItem>();
            Properties = new Dictionary<string, PropertyValue>();
            IncludeIndexNavigation = false;
        }

        public string GetSelectItemContentIdFromParameter()
        {
            if (SelectedMenuItem != null)
            {
                if (SelectedMenuItem.Callbacks.Count > 0)
                {
                    var callBack = SelectedMenuItem.Callbacks.First().Value;
                    foreach (var item in callBack.Parameters)
                    {
                        if (item.Key.ToLower() == "id")
                        {
                            return item.Value;
                        }
                    }
                }
            }

            return "";
        }


        /// <summary>
        /// Set the active flag for the Tab
        /// </summary>
        /// <param name="routeValueDictionary"></param>
        /// <param name="currentUri"></param>
        public void SetActiveMenuItem(System.Web.Routing.RouteValueDictionary routeValueDictionary, Uri currentUri)
        {
            foreach (var menuItem in MenuItems)
            {

                foreach (var callback in menuItem.Callbacks)
                {
                    if (routeValueDictionary["__px__routename"] != null && routeValueDictionary["__px__routename"].ToString().ToLowerInvariant() == callback.Value.RouteName.ToLowerInvariant())
                    {
                        menuItem.IsActive = true;
                        return;
                    }
                    else if (callback.Value.Controller == routeValueDictionary["controller"].ToString() && callback.Value.Action == routeValueDictionary["action"].ToString())
                    {
                        menuItem.IsActive = true;
                        return;
                    }
                    else
                    {
                        var isFoundController = false;
                        var isFoundAction = false;

                        foreach (var item in currentUri.AbsolutePath.Split('/'))
                        {
                            if (string.IsNullOrEmpty(item))
                                continue;

                            if (item.ToLowerInvariant() == callback.Value.Controller.ToLowerInvariant())
                            {
                                isFoundController = true;
                            }

                            if (item.ToLowerInvariant() == callback.Value.Action.ToLowerInvariant())
                            {
                                isFoundAction = true;
                            }

                            if (isFoundController || isFoundAction)
                            {
                                menuItem.IsActive = true;
                                return;
                            }

                            if (menuItem.Properties.ContainsKey("bfw_active_keywords"))
                            {
                                var bfwActiveKeyWords = menuItem.Properties["bfw_active_keywords"].Value.ToString();

                                if (bfwActiveKeyWords.Split(',').Contains(item))
                                {
                                    menuItem.IsActive = true;
                                    return;
                                } 
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(menuItem.Url) && menuItem.Url == currentUri.AbsolutePath)
                {
                    menuItem.IsActive = true;
                    return;
                }
            }
            if (MenuItems.Count > 0)
            {
                MenuItems[0].IsActive = true;
                return;
            }
        }
        public void SetActiveItem(List<MenuItem> menuItemsId, Models.Widget widget)
        {
            if (MenuItems.Count < 0)
            {
                MenuItems[0].IsActive = true;
                return;
            }
            else
            {
                foreach (var menuItem in menuItemsId)
                {
                    //if (string.IsNullOrEmpty(menuItem.Target))
                    //{
                        if (widget.Properties.ContainsKey("bfw_target_menu_id"))
                        {
                            if (menuItem.Id == widget.Properties["bfw_target_menu_id"].Value.ToString())
                            {
                                menuItem.IsActive = true;
                                return;
                            }
                        }
                    //}
                }

            }



        }

        public void RemoveMenuItemsBasedOnRole(DisplayOption currentUser)
        {
            var itemsToRemove = new List<MenuItem>();

            for (int index = MenuItems.Count - 1; index >= 0; index--)
            {
                bool isAllowed = false;
                var menuItem = MenuItems[index];
                foreach (var displayOption in menuItem.WidgetDisplayOptions.DisplayOptions)
                {
                    if (displayOption == currentUser)
                    {
                        isAllowed = true;
                        break;
                    }
                }

                if (!isAllowed)
                {
                    MenuItems.Remove(menuItem);
                }
            }
        }


    }
}