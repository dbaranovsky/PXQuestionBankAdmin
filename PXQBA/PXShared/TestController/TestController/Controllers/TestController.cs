using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Linq;

namespace Bfw.PX.PXPub.Controllers
{
    public class TestController : Controller
    {
        private const string PxModel = "Bfw.PX.PXPub.Models";

        [AllowCrossSiteJson]
        [ValidateInput(false)]
        public ActionResult RenderView(RenderViewInfo info)
        {
            if (string.IsNullOrEmpty(info.viewPath))
            {
                return new ContentResult() { Content = "Missing viewPath" };
            }

            object model = null;

            if (!string.IsNullOrEmpty(info.viewModelType))
            {
                Type type = GetModelType(info.viewModelType);

                if (type == null)
                {
                    return View("Cannot create type: " + info.viewModelType);
                }

                model = FormatterServices.GetUninitializedObject(type);

                if (model == null)
                {
                    return new ContentResult() { Content = "Invalid model type" };
                }

                if (info.viewModel != null)
                {
                    /* 
                     * The class customTypeConverter is for type conversion (e.g. {__type: "Bfw.PX.PXPub.Models.PxUnit, Bfw.PX.PXPub.Models"}) 
                     */
                    var customTypeConverter = new PxTypeConverter(); 
                    model = new JavaScriptSerializer(customTypeConverter).Deserialize(info.viewModel, type);
                }
            }

            SetDefaultViewData();
            SetViewData(info);

            return View(info.viewPath, model);
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        private Type GetModelType(string modelType)
        {
            var assembly = Assembly.Load(PxModel);
           
            var type = assembly.GetType(modelType) ?? Type.GetType(modelType);

            return type;
        }

        /// <summary>
        /// Sets specific ViewData if any
        /// </summary>
        /// <param name="info"></param>
        private void SetViewData(RenderViewInfo info)
        {
            if (info.viewData != null)
            {
                var entries = new JavaScriptSerializer().DeserializeObject(info.viewData) as Dictionary<string, object>;

                foreach (var entry in entries)
                {
                    var data = entry;

                    RenderViewData viewData = new RenderViewData();

                    foreach (var dataEntry in (data.Value as Dictionary<string, object>))
                    {
                        if (dataEntry.Key.Equals("dataType"))
                        {
                            viewData.dataType = dataEntry.Value.ToString();
                        }

                        if (dataEntry.Key.Equals("dataValue"))
                        {
                            viewData.dataValue = dataEntry.Value;
                        }
                    }

                    if (viewData.dataValue != null && viewData.dataType != null)
                    {
                        var assembly = Assembly.Load(PxModel);

                        Type type;

                        if (viewData.dataType.Contains("["))
                        {
                            var mainType = viewData.dataType.Split('[')[0];
                            var genericType = viewData.dataType.Split('[')[1].Replace("]", string.Empty);

                            type = Type.GetType(mainType).MakeGenericType(new Type[] { assembly.GetType(genericType) });

                            var dataValue = new JavaScriptSerializer().Serialize(viewData.dataValue);
                            ViewData[entry.Key] = new JavaScriptSerializer().Deserialize(dataValue, type);
                        }
                        else
                        {
                            Type theType = Type.GetType(viewData.dataType);
                            ViewData[entry.Key] = Convert.ChangeType(viewData.dataValue, theType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets commonly used objects (please add only those that are difficult to be passed thru json)
        /// </summary>
        private void SetDefaultViewData()
        {
            ViewData["TimeZones"] = new List<TimeZoneInfo>() { TimeZoneInfo.Local };
            ViewData["timeZone"] = TimeZoneInfo.Local;
        }
    }
}
