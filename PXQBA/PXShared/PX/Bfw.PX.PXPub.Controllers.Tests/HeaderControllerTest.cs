using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TestHelper;
using Bfw.PX.PXPub.Controllers;
using System;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class HeaderControllerTest
    {
        private IBusinessContext _context;
        private IPageActions _pageActions;
        private IUserActions _userActions;
        private HeaderController _controller;

        /// <summary>
        /// Basic init steps needed for all test methods as part of the arrangement 
        /// to help setup request context to act as helper for each test method
        /// </summary>
        
        [TestInitialize]
        public void InitializeControllerContext()
        {
            //Arrange
            _context = Substitute.For<IBusinessContext>();
            _pageActions = Substitute.For<IPageActions>();
            _userActions = Substitute.For<IUserActions>();


            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();
            
            // now go and setup Http request context e.g cookies, etc.
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("fff"));
            request.Cookies.ReturnsForAnyArgs(cookies);

            httpContext.Request.Returns(request); 

            var routeData = new RouteData();
            requestContext.RouteData = routeData;

            _controller = new HeaderController(_context, _pageActions, _userActions);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
                RequestContext = requestContext
            };

            _userActions.GetUser("3332").ReturnsForAnyArgs(new UserInfo() { FirstName = "abc", LastName = "bcd" });

            _controller.ControllerContext.RouteData.Returns(routeData);

        }

        [TestMethod]
        public void HeaderControllerTest_Verify_Top_Right_Banner_Is_Enabled_For_LearningCurve()
        {
            // this helps test the PX-6780 - The LC Dashboard is missing some options like instructor dropdown
            
            var agxCourse = new Bfw.Agilix.DataContracts.Course();
            agxCourse.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            _context.Course = agxCourse.ToCourse();
            _context.Course.CourseType = "LEARNINGCURVE";

            // we need to set a model property to help the specific 
            // view for LearningCurve to test logic for showing the right banner with drop-down
            // This tests issue PX-6780

            //Act
            var result = _controller.HomePageCourseHeader() as ViewResult;

            //Assert
            Assert.AreEqual("LEARNINGCURVE",((CourseHeader)result.Model).CourseType);
        }

        /// <summary>
        ///A test for HomePageCourseHeader
        /// Test whether appropriate values are set for a given timezone
        /// for reference, for "Pacific Standard Time" 
        /// Year	Start Date	End Date
        //2010	Sunday, March 14 at 2:00 AM	Sunday, November 7 at 2:00 AM
        //2011	Sunday, March 13 at 2:00 AM	Sunday, November 6 at 2:00 AM
        //2012	Sunday, March 11 at 2:00 AM	Sunday, November 4 at 2:00 AM
        //2013	Sunday, March 10 at 2:00 AM	Sunday, November 3 at 2:00 AM
        //2014	Sunday, March 9 at 2:00 AM	Sunday, November 2 at 2:00 AM
        //2015	Sunday, March 8 at 2:00 AM	Sunday, November 1 at 2:00 AM
        //2016	Sunday, March 13 at 2:00 AM	Sunday, November 6 at 2:00 AM
        //2017	Sunday, March 12 at 2:00 AM	Sunday, November 5 at 2:00 AM
        //2018	Sunday, March 11 at 2:00 AM	Sunday, November 4 at 2:00 AM
        //2019	Sunday, March 10 at 2:00 AM	Sunday, November 3 at 2:00 AM
        ///</summary>
        [TestMethod()]
        public void HeaderControllerTest_TimeZoneDate_Is_Correct_For_PST()
        {

            _context.Course.Returns(new Biz.DataContracts.Course()
            {
                Id = "courseId",
                CourseTimeZone = "Pacific Standard Time",
                CourseOwner = "3332"
            });

            var result = _controller.HomePageCourseHeader() as ViewResult;
            var model = result.Model;
            Assert.IsTrue(model is CourseHeader);
            var courseHeaderModel = model as CourseHeader;
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightOffset, -420);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardOffset, -480);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Year, DateTime.Now.Year);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Month, 11); 
            //The day of the daylight savings changes varies from year to year - we cant test this without importing the entire Timezone database or re-using the code under test
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Hour, 7); //2am PST == 7am GMT
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Year, DateTime.Now.Year);
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Month, 3);
            //The day of the daylight savings changes varies from year to year - we cant test this without importing the entire Timezone database or re-using the code under test
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Hour, 7); //2am PST == 7am GMT

            Assert.AreEqual(courseHeaderModel.TimeZoneAbbreviation,"PST");
            

        }

        [TestMethod()]
        public void HeaderControllerTest_TimeZoneDate_Is_Correct_For_Hawaiian_Standard_Time()
        {

            _context.Course.Returns(new Biz.DataContracts.Course()
            {
                Id = "courseId",
                CourseTimeZone = "Hawaiian Standard Time",
                CourseOwner = "3332"
            });

            var result = _controller.HomePageCourseHeader() as ViewResult;
            var model = result.Model;
            Assert.IsTrue(model is CourseHeader);
            var courseHeaderModel = model as CourseHeader;

            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightOffset, 0);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardOffset, -600);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Year, 1);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Month, 1);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTime.Hour, 0); 

            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Year, 1);
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Month, 1);
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTime.Hour, 0); 

            Assert.AreEqual(courseHeaderModel.TimeZoneAbbreviation, "HST");

        }

        [TestMethod()]
        public void HeaderControllerTest_TimeZoneDate_Is_Correct_For_NextYear()
        {

            _context.Course.Returns(new Biz.DataContracts.Course()
            {
                Id = "courseId",
                CourseTimeZone = "Pacific Standard Time",
                CourseOwner = "3332"
            });

            var result = _controller.HomePageCourseHeader() as ViewResult;
            var model = result.Model;
            Assert.IsTrue(model is CourseHeader);
            var courseHeaderModel = model as CourseHeader;
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTimeNextYear.Year, DateTime.Now.Year+1);
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTimeNextYear.Month, 11);
            //The day of the daylight savings changes varies from year to year - we cant test this without importing the entire Timezone database or re-using the code under test
            Assert.AreEqual(courseHeaderModel.TimeZoneStandardStartTimeNextYear.Hour, 7); //2am PST == 7am GMT
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTimeNextYear.Year, DateTime.Now.Year+1);
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTimeNextYear.Month, 3);
            //The day of the daylight savings changes varies from year to year - we cant test this without importing the entire Timezone database or re-using the code under test
            Assert.AreEqual(courseHeaderModel.TimeZoneDaylightStartTimeNextYear.Hour, 7); //2am PST == 7am GMT

            Assert.AreEqual(courseHeaderModel.TimeZoneAbbreviation, "PST");


        }
    }
}
