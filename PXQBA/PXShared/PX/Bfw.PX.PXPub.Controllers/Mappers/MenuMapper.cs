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
    public static class MenuMapper
    {
        public static Menu ToMenu(this BizDC.Menu biz, bool noSequence = false)
        {
            var model = new Menu();

            if (biz != null)
            {
                model.Id = biz.Id;
                model.ParentId = biz.ParentId;
                model.CourseID = biz.CourseID;
                model.Sequence = biz.Sequence;
                model.Title = biz.Title;
                model.BfwTocId = biz.BfwTocId;
                model.FlatCount = biz.FlatCount;

                var buider = new StringBuilder();

                if (!noSequence)
                {
                    foreach (var menuItem in biz.MenuItems.OrderBy(mi => mi.Sequence).ToList())
                    {
                        model.MenuItems.Add(menuItem.ToMenuItem());
                    }
                }
                else
                {
                    foreach (var menuItem in biz.MenuItems)
                    {
                        model.MenuItems.Add(menuItem.ToMenuItem());
                    }
                }

                foreach (var menuItem in biz.MenuItemTemplates.OrderBy(mi => mi.Sequence).ToList())
                {
                    model.MenuItemTemplates.Add(menuItem.ToMenuItem());
                }

                foreach (var key in biz.Properties.Keys)
                {
                    model.Properties.Add(key, biz.Properties[key]);
                }
            }

            return model;
        }

        public static BizDC.Menu ToMenu(this Menu model)
        {
            var biz = new BizDC.Menu();

            if (model == null)
            {
                return biz;
            }

            biz.Id = model.Id;
            biz.ParentId = model.ParentId;
            biz.CourseID = model.CourseID;
            biz.Sequence = model.Sequence;
            biz.Title = model.Title;                        

            foreach (var menuItem in model.MenuItems)
            {
                biz.MenuItems.Add(menuItem.ToMenuItem());
            }

            foreach (var menuItem in model.MenuItemTemplates)
            {
                biz.MenuItemTemplates.Add(menuItem.ToMenuItem());
            }

            return biz;
        }

        public static MenuItem ToMenuItem(this BizDC.MenuItem biz)
        {
            var model = new MenuItem();

            if (biz != null)
            {
                model.Abbreviation = biz.Abbreviation;
                model.BfwSubType = biz.BfwSubType;
                model.BfwCssClass = biz.BfwCssClass;
                model.BfwType = biz.BfwType;
                model.Id = biz.Id;
                model.ParentId = biz.ParentId;
                model.CourseID = biz.CourseID;
                model.Sequence = biz.Sequence;
                model.FlatSequence = biz.FlatSequence;
                model.MenuSequence = biz.MenuSequence;
                model.IsHidden = biz.IsVisible;
                if (biz.MenuItems != null)
                {
                    model.MenuItems = new List<MenuItem>();

                    foreach (var menuitem in biz.MenuItems)
                    {
                        model.MenuItems.Add(menuitem.ToMenuItem());
                    }
                }
                model.Title = biz.Title;
                model.Type = biz.Type;
                model.BfwMenuCreatedby = biz.BfwMenuCreatedby;
                model.BfwDisplayOnProductCourse = biz.BfwDisplayOnProductCourse;
                model.SelectedByDefault = biz.SelectedByDefault;

                foreach (var callback in biz.Callbacks)
                {
                    model.Callbacks.Add(callback.Key, callback.Value.ToMenuItemCallback());
                }

                if (!model.Callbacks.IsNullOrEmpty())
                {
                    model.Action = model.Callbacks.First().Value.Action;
                    model.Controller = model.Callbacks.First().Value.Controller;
                    model.Url = model.Callbacks.First().Value.Url;
                    model.Target = model.Callbacks.First().Value.Target;
                    model.Parameters = model.Callbacks.First().Value.Parameters;
                }

                
                model.IsDisabled = biz.IsDisabled;
                


                foreach (var key in biz.Properties.Keys)
                {
                    model.Properties.Add(key, biz.Properties[key]);
                }

                model.WidgetDisplayOptions = biz.WidgetDisplayOptions.ToWidgetDisplayOptions();

                foreach (var displayOption in model.WidgetDisplayOptions.DisplayOptions)
                {
                    if (displayOption == BizDC.DisplayOption.Student)
                    {
                        model.VisibleByStudent = true;
                    }

                    if (displayOption == BizDC.DisplayOption.Instructor)
                    {
                        model.VisibleByInstructor = true;
                    }
                }
            }

            return model;
        }

        public static BizDC.MenuItem ToMenuItem(this MenuItem model)
        {
            var biz = new BizDC.MenuItem();

            if (model != null)
            {
                biz.Abbreviation = model.Abbreviation;
                biz.BfwSubType = model.BfwSubType;
                biz.BfwCssClass = model.BfwCssClass;
                biz.BfwType = model.BfwType;
                biz.Id = model.Id;
                biz.ParentId = model.ParentId;
                biz.CourseID = model.CourseID;
                biz.Sequence = model.Sequence;
            
                biz.BfwMenuCreatedby = model.BfwMenuCreatedby;
                biz.Title = model.Title;
                biz.Type = model.Type;
                biz.IsVisible = model.IsHidden;


                biz.Callbacks.Add( "first", new BizDC.MenuItemCallback() { Url = model.Url });
                biz.WidgetDisplayOptions = new BizDC.WidgetDisplayOptions();

                if (model.VisibleByStudent)
                {
                    biz.WidgetDisplayOptions.DisplayOptions.Add(BizDC.DisplayOption.Student);
                }

                if (model.VisibleByInstructor)
                {
                    biz.WidgetDisplayOptions.DisplayOptions.Add(BizDC.DisplayOption.Instructor);
                }

                foreach (var key in model.Properties.Keys)
                {
                    biz.Properties.Add(key, model.Properties[key]);
                }

                //biz.WidgetDisplayOptions = model.WidgetDisplayOptions.ToWidgetDisplayOptions();
            }

            return biz;
        }
    }
}
