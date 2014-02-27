using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class WidgetMapper
    {
        public static Widget ToWidgetItem(this  BizDC.Widget biz)
        {
            var model = new Widget();

            if (biz != null)
            {
                model.Abbreviation = biz.Abbreviation;
                model.BfwSubType = biz.BfwSubType;
                model.BfwType = biz.BfwType;
                model.Id = biz.Id;
                model.ParentId = biz.ParentId;
                model.EntityID = biz.CourseID;
                model.Sequence = biz.Sequence;
                model.Template = biz.Template;
                model.Title = biz.Title;
                model.Type = biz.Type;
                model.UseProductCourse = biz.UseProductCourse;
                model.IsMultipleAllowed = biz.IsMultipleAllowed;
                model.IsCollapseAllowed = biz.IsCollapseAllowed;
                model.IsTitleHidden = biz.IsTitleHidden;
                model.ListStudents = biz.ListStudents;
                model.IsShowPersistentQtip = biz.IsShowPersistentQtip;
                model.PersistentQtips = biz.PersistentQtips;

                foreach (var callback in biz.Callbacks)
                {
                    model.Callbacks.Add(callback.Key, callback.Value.ToWidgetCallback());
                }

                foreach (var inputHelper in biz.InputHelpers)
                {
                    model.InputHelpers.Add(inputHelper.Key, inputHelper.Value.ToWidgetInputHelper());
                }

                foreach (var key in biz.Properties.Keys)
                {
                    model.Properties.Add(key, biz.Properties[key]);
                }

                foreach (var key in biz.BHProperties.Keys)
                {
                    model.BHProperties.Add(key, biz.BHProperties[key]);
                }

                model.WidgetDisplayOptions = biz.WidgetDisplayOptions.ToWidgetDisplayOptions();
            }

            return model;
        }

        public static BizDC.Widget ToWidgetItem(this Widget model)
        {
            var biz = new BizDC.Widget();

            if (model != null)
            {
                biz.Abbreviation = model.Abbreviation;
                biz.BfwSubType = model.BfwSubType;
                biz.BfwType = model.BfwType;
                biz.Id = model.Id;
                biz.ParentId = model.ParentId;
                biz.CourseID = model.EntityID;
                biz.Sequence = model.Sequence;
                biz.Template = model.Template;
                biz.Title = model.Title;
                biz.Type = model.Type;

                foreach (var callback in model.Callbacks)
                {
                    biz.Callbacks.Add(callback.Key, callback.Value.ToWidgetCallback());
                }

                foreach (var key in model.Properties.Keys)
                {
                    biz.Properties.Add(key, model.Properties[key]);
                }

                if (model.BHProperties != null)
                {
                    foreach (var key in model.BHProperties.Keys)
                    {
                        biz.BHProperties.Add(key, model.BHProperties[key]);
                    }
                }
                biz.WidgetDisplayOptions = model.WidgetDisplayOptions.ToWidgetDisplayOptions();
            }

            return biz;
        }
    }
}
