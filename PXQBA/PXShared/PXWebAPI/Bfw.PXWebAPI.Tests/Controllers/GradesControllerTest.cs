using NSubstitute;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Controllers;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using System.Web.Script.Serialization;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Bfw.PXWebAPI.Tests
{
    [TestClass]
    public class GradesControllerTest
    {
        private GradesController controller;

        private ISessionManager sessionManager;
        private IBusinessContext context;
        private IApiGradeBookActions gradebookActions;
        private IApiGradeActions gradeActions;
        private IApiItemActions itemActions;

        private HttpContext httpContext;

        [TestInitialize]
        public void TestInitialize()
        {

            sessionManager = Substitute.For<ISessionManager>();
            context = Substitute.For<IBusinessContext>();
            gradebookActions = Substitute.For<IApiGradeBookActions>();
            gradeActions = Substitute.For<IApiGradeActions>();
            itemActions = Substitute.For<IApiItemActions>();

            controller = new GradesController(sessionManager, context, gradebookActions, gradeActions, itemActions);

            InitializeControllerContext();      
        }

        [TestMethod]
        public void Details_Should_Return_Empty_Response()
        {
            var result = controller.Details("1");

            Assert.AreEqual(Helper.NO_RESULTS, result.error_message);
        }

        [TestMethod]
        public void Details_Should_Return_Full_Structure()
        {
            var enrollments = new List<Enrollment>()
            {
                new Enrollment()
                {
                    User = new AgilixUser()
                    {
                        Id = "2"
                    }
                }
            };

            var grades = new List<Grade>()
            {
                new Grade()
                {                    
                }
            };

            var item = new Item()
            {
                Id = "3",
                Category = "4"
            };

            var gradeBookWeights = new GradeBookWeights()
            {
            };

            var gradeWeightCategories = new List<GradeBookWeightCategory>()
            {
                new GradeBookWeightCategory()
                {
                    Id = "4",
                    Text = "Uncategorized"
                }
            };

            grades.First().GetType().GetProperty("Item").SetValue(grades.First(), item, null);
            enrollments.First().GetType().GetProperty("ItemGrades").SetValue(enrollments.First(), grades, null);
            gradeBookWeights.GetType().GetProperty("GradeWeightCategories").SetValue(gradeBookWeights, gradeWeightCategories, null);

            sessionManager.CurrentSession.When(s => s.Execute(Arg.Any<GetGrades>())).Do(s => {
                s.Arg<GetGrades>().GetType().GetProperty("Enrollments").SetValue(s.Arg<GetGrades>(), enrollments, null);
            });

            itemActions.GetItems("1", "3").Returns(new List<Item>() { item });
            gradebookActions.GetGradeBookWeights("1").Returns(gradeBookWeights);

            var result = controller.Details("1");
            string expected = "{\"results\":[{\"ItemId\":\"3\",\"DueDate\":null,\"MaxPoints\":0,\"Title\":null,\"Description\":null,\"Category\":\"Uncategorized\",\"Visibility\":true,\"Sequence\":null,\"iconUri\":\"\",\"ParentId\":null,\"SubContainerId\":null,\"Type\":\"None\",\"SubType\":\"\",\"CourseId\":null,\"UserGrades\":[{\"UserId\":\"2\",\"Score\":0,\"ScoredDate\":null,\"Status\":\"None\",\"Duration\":0}]}],\"status_code\":0,\"error_message\":null}";

            Assert.AreEqual(expected, new JavaScriptSerializer().Serialize(result));
        }

        private void InitializeControllerContext()
        {
            var httpRequest = new HttpRequest("", "http://url/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null).Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }
    }
}
