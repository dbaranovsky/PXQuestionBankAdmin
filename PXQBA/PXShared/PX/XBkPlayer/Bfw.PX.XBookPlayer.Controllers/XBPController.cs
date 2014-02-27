using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bfw.PX.XBkPlayer.Biz.DataContracts;
using Bfw.PX.XBkPlayer.Biz.Services;
using Bfw.PX.XBkPlayer.Models;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System;

namespace Bfw.PX.XBkPlayer.Controllers
{
    public class XBPController:Controller
    {
        private bool isActive = false;
        private bool isReview = false;
        private bool isPrint = false;
        private bool isPrintKey = false;
        private bool isNone = true;
        private string encryptKey = "";
        private string baseUrl = "";

        public IXBPServices XBkServices { get; set; }

        public XBPController(IXBPServices xbkServices)
        {
            XBkServices = xbkServices;
            encryptKey = System.Configuration.ConfigurationManager.AppSettings["EncryptionKey"];
            baseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"];
        }

        public ActionResult Index()
        {
            XBkModel model = new XBkModel(Request);
            model.XBkJsonResult = new JavaScriptSerializer().Serialize(model);
            ViewData.Model = model;

            return View();
        }

        public ActionResult LoadXBkPlayer()
        {
            string questionId="", entityId="", enrollmentId="", serverUrl="";

            if (!string.IsNullOrEmpty(Request.QueryString["entityId"]))
                entityId = Request.QueryString["entityId"].ToString();

            if (!string.IsNullOrEmpty(Request.QueryString["questionId"]))
                questionId = Request.QueryString["questionId"].ToString();

            if (!string.IsNullOrEmpty(Request.QueryString["ServerUrl"]))
                serverUrl = Request.QueryString["ServerUrl"].ToString();

            var question = XBkServices.GetQuestion(entityId, questionId);

            var data = new XBkPlayerData()
            {
                EntityId = entityId,
                EnrollmentId = enrollmentId,
                QuestionId = questionId,
                QuestionBody = question.Body.ToString()
            };

            data.LoadQuestion(question.QuestionXml);
            return View("QuestionPlayer", data);
        }

        [HttpPost]
        public string GetQuestionResponse()
        {
            bool err = false;
            string strIn = new StreamReader(Request.InputStream, Request.ContentEncoding).ReadToEnd();

            string partId = "";
            double possible = 0;
            double computed = 0;

            XDocument request = null;
            try
            {
                request = XDocument.Load(new System.IO.StringReader(strIn));

                partId = request.Root.Element("submission").Attribute("partid").Value;
                possible = System.Xml.XmlConvert.ToDouble(request.XPathSelectElement("info/response").Attribute("pointspossible").Value);

            }
            catch
            {
                partId = "Bad format of request";
                err = true;
            }

            if (!err)
            {
                try
                {
                    computed = GetScore(request.Root, possible);
                }
                catch
                {
                    partId = "Error while processing request";
                    err = true;
                }
            }

            XDocument custom = new XDocument(new XElement("custom", new XElement("response", new XAttribute("type", "submission"), new XAttribute("foreignid", partId),
                               new XAttribute("pointspossible", System.Xml.XmlConvert.ToString(possible)), new XAttribute("pointscomputed", System.Xml.XmlConvert.ToString(computed))
                            )));

            return custom.ToString();
        }

        private double GetScore(XElement info, double possible)
        {
            double rawScore = .0;
            try
            {
                string problemXML = info.XPathSelectElement("question/interaction/data").Value;
                string answer = info.Element("submission").Element("answer").Value;

                //CHTSProblem hts = new CHTSProblem(mathJS, "");
                //rawScore = hts.doScore(problemXML, answer, encryptKey, getSignKey(info));
                rawScore /= 100;

            }
            catch (Exception ex)
            {
            }
            // Always scale the points to possible
            return rawScore * possible;
        }

        [HttpPost]
        public string GetCustomQuestion()
        {
            string baseUrl = "";
            string strIn = new StreamReader(Request.InputStream, Request.ContentEncoding).ReadToEnd();

            var doc = XDocument.Load(new StringReader(strIn));
            var data = new XBkPlayerData();

            data.LoadQuestion(GetProblemXML(doc.Root));

            var strout = RenderViewToString("QuestionPlayer", data);

            var customDoc = new XDocument(new XElement("custom",
                new XElement("version", "1"),
                new XElement("score", baseUrl + "GetQuestionResponse"),
                new XElement("display", "Active,Review,Print,PrintKey"),
                new XElement("privatedata", getSignKey(doc.Root)), new XElement("body", new XCData(strout))));

            return customDoc.ToString();
        }

        private string GetProblemXML(XElement info)
        {
            //temporary code
            return "<info id='0' mode='Active'><question questionid='b8869dc4-ac2b-42a3-9127-385a1ee04b5a' version='0.0.1' resourceentityid='89914,0,0,0' creationdate='2012-11-28T21:02:39.14Z' creationby='86734' modifieddate='2012-11-28T21:02:39.14Z' modifiedby='86734' flags='6' actualentityid='89914' schema='2' partial='false'><meta><createdBy>86734</createdBy><userCreated>true</userCreated><totalUsed>1</totalUsed></meta><answer><value></value></answer><body></body><interaction type='essay' height='200' /></question></info>";
            //

            string problemXML = "";
            try
            {
                problemXML = info.XPathSelectElement("question/interaction/data").Value;
            }
            catch { problemXML = "<div>Bad request: cannot read .../interaction/data element</div>"; }
            return problemXML.ToString();
        }


        private string getSignKey(XElement info)
        {
            var problemXMLguid = System.Guid.NewGuid(); 

            string signKey = "";
            try
            {
                XElement node = info.XPathSelectElement("submission/attemptquestion/customprivatedata");
                if (node != null)
                {
                    signKey = node.Value;
                }
            }
            catch { }
            if (signKey == "")
            {
                signKey = problemXMLguid.ToString();
            }

            return signKey;
        }

        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
