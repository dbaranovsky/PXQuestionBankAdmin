using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Xml.Linq;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class NavigationItemMapper
    {
        /// <summary>
        /// Maps a NavigationItem business object tot a NavigationItem model.
        /// </summary>
        /// <param name="biz">The NavigationItem business object.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// NavigationItem model.
        /// </returns>
        public static Models.NavigationItem ToNavigationItem(this BizDC.NavigationItem biz, BizSC.IContentActions content)
        {
            Models.NavigationItem model = null;

            if (null != biz)
            {
                model = new Models.NavigationItem
                {
                    Id = biz.Id,
                    ParentId = biz.ParentId,
                    Title = biz.Name,
                    Type = biz.Type,
                    Items = biz.Children.Map(ni => ni.ToNavigationItem(content)).ToList(),
                    Highlighted = biz.Highlilghted,
                    Sequence = biz.Sequence,
                    ExtendedLinkType = biz.Properties.ContainsKey("bfw_extendedlinktype")
                                           ? biz.Properties["bfw_extendedlinktype"].Value.ToString()
                                           : ""
                };

                if (!biz.Categories.IsNullOrEmpty())
                {
                    model.Categories = biz.Categories.Map(cat => cat.ToTocCategory());
                }

                if (!biz.Links.IsNullOrEmpty())
                {
                    foreach (var item in biz.Links.Map(i => i.ToLink()).ToList())
                    {
                        model.Children.Add(item);
                    }
                }

                if (!model.Items.IsNullOrEmpty())
                {
                    foreach (var item in model.Items)
                    {
                        model.Children.Add(item);
                    }
                }

                model.Children = model.Children.OrderBy(mi => mi.Sequence).ToList();
                model.Visibility = XElement.Parse(biz.Visibility, LoadOptions.None);
                model.TocId = biz.TocId;

                try
                {
                    model.IsDisplayTitle = biz.IsDisplayTitle;
                    model.TitleURL = biz.TitleURL;
                    model.IsSupportAddLink = biz.IsSupportAddLink;
                    model.IsSupportAddMenu = biz.IsSupportAddMenu;
                    model.DisplayTitle = string.IsNullOrEmpty(biz.DisplayTitle) ? biz.Name : biz.DisplayTitle;
                }
                catch { }
            }

            return model;
        }

        /// <summary>
        /// Convert to a Navigation item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static BizDC.NavigationItem ToNavigationItem(this Models.NavigationItem model, BizSC.IContentActions content)
        {
            BizDC.NavigationItem biz = null;

            if (null != model)
            {
                biz = new BizDC.NavigationItem();
                biz.Id = model.Id;
                biz.ParentId = model.ParentId;
                biz.Name = model.Title;
                biz.Type = model.Type;
                biz.Children = model.Items.Map(ni => ni.ToNavigationItem(content)).ToList();
                biz.Highlilghted = model.Highlighted;
                biz.Sequence = model.Sequence;
                biz.Visibility = model.Visibility.ToString();

            }

            return biz;
        }

        /// <summary>
        /// Converts to a Biz.ContentItem from a Navigation Item Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Models.NavigationItem model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);
            if (null != model)
            {
                biz.Type = "Folder";
                biz.Subtype = "NavigationItem";
                biz.ParentId = model.ParentId;
                biz.Properties["highlighted"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.Highlighted };
                biz.Properties["bfw_istoplevel"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsTopLevel };
                biz.Properties["bfw_isactive"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsActive };
                biz.Properties["bfw_tocid"] = new PropertyValue { Type = PropertyType.String, Value = model.TocId };
                biz.Properties["bfw_isfne"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsFnE };
                biz.Properties["bfw_visibility"] = new PropertyValue { Type = PropertyType.String, Value = model.Visibility.ToString() };

                biz.Properties["bfw_titleurl"] = new PropertyValue { Type = PropertyType.String, Value = model.TitleURL };
                biz.Properties["bfw_issupportaddlink"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsSupportAddLink };
                biz.Properties["bfw_issupportaddmenu"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsSupportAddMenu };
                biz.Properties["bfw_isdisplaytitle"] = new PropertyValue { Type = PropertyType.Boolean, Value = model.IsDisplayTitle };
                biz.Properties["bfw_displaytitle"] = new PropertyValue { Type = PropertyType.String, Value = model.DisplayTitle };

                biz.Properties["bfw_extendedlinktype"] = new PropertyValue // This was added to set active menu on the header menu.
                {
                    Type = PropertyType.String,
                    Value = "custom_" + model.TocId.ToLowerInvariant()
                };

                biz.AssignmentSettings = new AssignmentSettings()
                {
                    IsAssignable = true,
                    DropBoxType = DropBoxType.None,
                    DueDate = model.DueDate,
                    Points = 0,
                    Category = ""
                };

                if (model.Categories.IsNullOrEmpty())
                {
                    biz.DefaultCategoryParentId = biz.ParentId;
                    biz.DefaultCategorySequence = biz.Sequence;
                }
            }

            return biz;
        }
    }
}
