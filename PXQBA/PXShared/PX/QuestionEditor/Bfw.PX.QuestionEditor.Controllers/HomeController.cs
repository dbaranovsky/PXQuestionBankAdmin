using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using System.Xml;
using Bfw.PX.QuestionEditor.Models;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using Bfw.PX.QuestionEditor.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace QuestionEditor.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        ///// <summary>
        ///// Access to an IContentActions implementation.
        ///// </summary>
        ///// <value>
        ///// The content actions.
        ///// </value>
        //protected BizSC.IBusinessContext Context { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public HomeController()
        {
        }

        public HomeController(BizSC.IContentActions contentActions)
        {
            ContentActions = contentActions;
            //Context = context;
        }

        public ActionResult LoadHtsData()
        {
            string questionId, entityId;
            //TODO: remove temporary Ids
            if (Request.QueryString["Id"] != null)
            {
                entityId = Request.QueryString["EntityId"].ToString();
                questionId = Request.QueryString["Id"].ToString();
            }
            else
            {
                entityId = "41306";
                questionId = "F92C8DF3081C40339B07999B50C13ACA";
            }

            var question = ContentActions.GetQuestion(entityId, questionId).InteractionData;

            var htsData = new HTSData();
//            var path = Server.MapPath("/") + "/xml/roget_aq_01_01_023.xml";
//            htsData.LoadFromFile(path);

            htsData.LoadFromString(question, questionId, entityId);
            
            return Content(JsonHelper.Serialize(htsData.Steps), "application/json; charset=utf-8");
        }

        public ActionResult SubmitHtsData(string htsData)
        {
            var obj = JsonHelper.Deserialize<HTSData>(htsData);
            
            return Content("done");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Summary()
        {
            return View();
        }

        public class JsonHelper
        {
            public static string Serialize<T>(T obj)
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                var ms = new MemoryStream();
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }

            public static T Deserialize<T>(string json)
            {
                T obj = Activator.CreateInstance<T>();
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
                return obj;
            }
        }
    }
}
