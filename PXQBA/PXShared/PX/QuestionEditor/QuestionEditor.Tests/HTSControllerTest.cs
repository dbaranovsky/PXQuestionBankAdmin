using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.QuestionEditor.Biz.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using QuestionEditor.Controllers;
using BizSC = Bfw.PX.QuestionEditor.Biz.Services;


namespace QuestionEditor.Tests
{
    [TestClass]
    public class HTSControllerTest
    {
        private BizSC.IHTSServices _htsServices;
        private HTSController _htsController;
        

        [TestInitialize]
        public void TestInitialize()
        {
            _htsServices = Substitute.For<BizSC.IHTSServices>();
            _htsController = new HTSController(_htsServices);
        }


        [TestMethod]
        public void SolutionFieldShouldBeEmptyForConvertedQuestions()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();

            var queryStrings = new NameValueCollection
            {
                {"questionId", "dummy"}, 
                {"convert", "true"}
            };

            request.QueryString.ReturnsForAnyArgs(queryStrings);
            
            var requestContext = Substitute.For<RequestContext>();
            httpContext.Request.Returns(request);
            _htsController.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };

            _htsServices.GetQuestion("entityid", "questionId").ReturnsForAnyArgs(
                    new Question()
                    {
                        Body = "Dummy Question"
                    }
                );

            var viewResult = _htsController.LoadHtsData() as ContentResult;
            var content = new JavaScriptSerializer().Deserialize<HTSData>(viewResult.Content);
            Assert.AreEqual("", content.Solution,"Solution Field is not empty for converted questions.");
        }
    }
}
