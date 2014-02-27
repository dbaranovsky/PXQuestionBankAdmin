using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Web.Mvc;
using NSubstitute;
using TestHelper;
using Bfw.PX.Biz.DataContracts;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    
    
    /// <summary>
    ///This is a test class for AboutCourseWidgetControllerTest and is intended
    ///to contain all AboutCourseWidgetControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AboutCourseWidgetControllerTest
    {


        private TestContext testContextInstance;
        private IBusinessContext _context;
        private IContentActions _contentActions;
        private IPageActions _pageActions;
        private ICourseActions _courseActions;
        private IUserActions _userActions;
        private AboutCourseWidgetController _controller;
        private IUrlHelperWrapper _urlHelper;
        private IServiceLocator _serviceLocator;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();
            _pageActions = Substitute.For<IPageActions>();
            //_helper = Substitute.For<ContentHelper>();
            _courseActions = Substitute.For<ICourseActions>();
            _userActions = Substitute.For<IUserActions>();
            _urlHelper = Substitute.For<IUrlHelperWrapper>();
            _serviceLocator = Substitute.For<IServiceLocator>();
            _serviceLocator.GetInstance<IUrlHelperWrapper>().Returns(_urlHelper);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);


            _controller = new AboutCourseWidgetController(_context, _contentActions, _pageActions, null, _courseActions, _userActions, _urlHelper);

         
        }
        
        
        #endregion


        /// <summary>
        ///UpdateLMSIDTest should return a JSON object with success
        ///</summary>
        [TestMethod()]
        public void UpdateLMSID_returns_success()
        {
            var user = new UserInfo()
            {
                Id = "id",
                Username = "user1",
                ReferenceId = "lmsidOld",
                DomainId = "domain1"
            };
            _context.CurrentUser.Returns(user);
            string lmsId = "lmsidNew";
            var expected = new
                    {
                        success = true,
                        LmsId = "lmsidNew",
                        userId = "id"
                    };
            

            _userActions.UpdateUser(user).Returns(true);
            var result = _controller.UpdateLMSID(lmsId);
            
            Assert.AreEqual(expected.ToString(), result.Data.ToString());
        }

        /// <summary>
        ///UpdateLMSIDTest should return a JSON object with success = false when lmsid is blank
        ///</summary>
        [TestMethod()]
        public void UpdateLMSID_returns_fail_blank_id()
        {
            string lmsId = "";
            var expected = new
            {
                success = false,
            };

            var result = _controller.UpdateLMSID(lmsId);

            Assert.AreEqual(expected.ToString(), result.Data.ToString());
        }

        /// <summary>
        ///UpdateLMSID should update the user with new lmsid
        ///</summary>
        [TestMethod()]
        public void UpdateLMSID_calls_updateUser()
        {
            var user = new UserInfo()
          {
                Id = "id",
                Username = "user1",
                ReferenceId = "lmsidOld",
                DomainId = "domain1"
            };
            _context.CurrentUser.Returns(user);
            string lmsId = "lmsidNew";
            bool updateUserCalled = false;

            _userActions.UpdateUser(Arg.Is<UserInfo>(u => u.ReferenceId == "lmsidNew")).Returns(true).AndDoes(ci => updateUserCalled = true);
            
            _controller.UpdateLMSID(lmsId);
            
            Assert.IsTrue(updateUserCalled);
        }

        /// <summary>
        ///UpdateLMSID should update the user with new lmsid
        ///</summary>
        [TestMethod()]
        public void AboutCourseWidget_View_Returns_Proper_SyllabusUrl()
        {
            var httpContext = Substitute.For<HttpContextBase>();            
            var requestContext = Substitute.For<RequestContext>();
            const string courseId = "11111";
            string syllabus = string.Format("{0}/CourseSyllabusSpring2014.doc", courseId);
            string downloadURL = string.Format("Download/DownloadEnrollmentFile?resourcePath={0}&entityId={1}", syllabus, courseId);
            const string widgetId = "123123";
                        
            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };                       
            _controller.Request.QueryString.Returns(new NameValueCollection());

            _urlHelper.Action("DownloadEnrollmentFile", "Download", Arg.Is<object>(x => x.GetType().GetProperty("resourcePath").GetValue(x, null).ToString() == syllabus && x.GetType().GetProperty("entityId").GetValue(x, null).ToString() == courseId)).Returns(downloadURL); 
            
            var user = new UserInfo()
            {
                Id = "1",
                Username = "user1",
                ReferenceId = "2",
                DomainId = "3"
            };
            _context.CurrentUser.Returns(user);
            
            _context.CourseId.Returns(courseId);
            var course = new Course
            {
                Id = courseId,
                Syllabus = syllabus,
                SyllabusType = "File",
                CourseType = "Launchpad",
                ContactInformation = new List<ContactInfo> {new ContactInfo() {ContactType = "", ContactValue = ""}}
            };
            _context.Course.Returns(course);        
            _courseActions.GetCourseByCourseId(courseId).Returns(course);

            _pageActions.GetWidget(widgetId).Returns(new Widget { Id = widgetId, Properties = new Dictionary<string, PropertyValue>() });

            var actionResult = _controller.View(new Models.Widget { Id = widgetId }) as PartialViewResult;
            
            var model = actionResult.ViewData.Model as Models.AboutCourse;

            Assert.IsNotNull(model);
            Assert.AreEqual(downloadURL, model.SyllabusUrl);
        }

    }
}
