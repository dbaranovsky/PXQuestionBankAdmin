using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class WidgetConfigurationMapper
    {

        /// <summary>
        /// Convert to a Web Configuration.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static WidgetConfiguration ToWebConfiguration(this BizDC.ContentItem biz)
        {
            var model = new WidgetConfiguration();

            model.Id = biz.Id;
            model.Title = biz.Title;
            model.ParentId = biz.ParentId;
            model.Sequence = biz.Sequence;
            model.Url = biz.Href;

            model.Action = biz.Properties.ContainsKey("bfw_action") ? biz.Properties["bfw_action"].Value.ToString() : "";
            model.Controller = biz.Properties.ContainsKey("bfw_controller") ? biz.Properties["bfw_controller"].Value.ToString() : "";

            if (biz.Properties.ContainsKey("bfw_visibility"))
            {
                var value = biz.Properties.ContainsKey("bfw_visibility") ? biz.Properties["bfw_visibility"].Value.ToString() : "";
                model.Visibility = XElement.Parse(value, LoadOptions.None);
            }
            else
            {
                model.Visibility = XElement.Parse("<bfw_visibility />", LoadOptions.None);
            }

            model.IsCollapsed = biz.Properties.ContainsKey("bfw_iscollapsed") && bool.Parse(biz.Properties["bfw_iscollapsed"].Value.ToString());
            model.IsTitleHidden = biz.Properties.ContainsKey("bfw_istitlehidden") && bool.Parse(biz.Properties["bfw_istitlehidden"].Value.ToString());
            model.ListStudents = biz.Properties.ContainsKey("bfw_liststudents") && bool.Parse(biz.Properties["bfw_liststudents"].Value.ToString());
            model.IsViewAllSupported = biz.Properties.ContainsKey("bfw_isviewallsupported") && bool.Parse(biz.Properties["bfw_isviewallsupported"].Value.ToString());
            model.IsVisible = !biz.Properties.ContainsKey("bfw_isvisible") || bool.Parse(biz.Properties["bfw_isvisible"].Value.ToString());
            model.ViewAllFne = !biz.Properties.ContainsKey("bfw_viewallfne") || bool.Parse(biz.Properties["bfw_viewallfne"].Value.ToString());
            model.TargetId = biz.Properties.ContainsKey("bfw_targetid") ? biz.Properties["bfw_targetid"].Value.ToString() : "";
            return model;
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from a WidgetConfiguration Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this WidgetConfiguration model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Custom";
                biz.Properties["bfw_type"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = "WidgetConfiguration" };
                biz.Properties["bfw_controller"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.Controller };
                biz.Properties["bfw_action"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.Action };
                biz.Properties["bfw_cssclass"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.CssClass };
                biz.Properties["bfw_viewallfne"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.ViewAllFne };
                biz.Properties["bfw_visibility"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.Visibility.ToString() };
                biz.Properties["bfw_iscollapsed"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.Boolean, Value = model.IsCollapsed };
                biz.Properties["bfw_istitlehidden"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.Boolean, Value = model.IsTitleHidden };
                biz.Properties["bfw_isviewallsupported"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.Boolean, Value = model.IsViewAllSupported };
                biz.Properties["bfw_isvisible"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.Boolean, Value = model.IsVisible };
                biz.Properties["bfw_targetid"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.TargetId };
                biz.Properties["bfw_liststudents"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.ListStudents };
            }

            return biz;
        }        
    }
}
