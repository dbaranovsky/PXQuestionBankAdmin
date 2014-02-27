using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using Bfw.PX.PXPub.Controllers.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Caching;
using Helper = TestHelper.Helper;
using System.Configuration;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
	[TestClass]
	public class ExternalContentMapperTests
	{
		private IBusinessContext _context;
		private UserInfo userInfo;
		private Type externalContentMapperType;
        private ExternalDomainController _externalDomainController;


		[TestInitialize]
		public void TestInitialize()
		{
            _context = Substitute.For<IBusinessContext>();

			const string tstUrl = "http://lcl.worthpublishers.com/launchpad/myers10e/1/Dashboard";
	
			userInfo = new UserInfo {ReferenceId = "1"};

            _context.CurrentUser = userInfo;

			var course = new Bfw.Agilix.DataContracts.Course();
			course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
			course.Id = "1";
            _context.Course = course.ToCourse();
            _context.Course.EnableArgaUrlMapping = true;
            _context.Product = new Biz.DataContracts.Course();

			var domain = new PX.Biz.DataContracts.Domain() { Id = "8841", Name = "Default", Userspace = "bfwusers"};

            _context.Domain.Returns(domain);
		    _context.URL = "http://lcl.whfreeman.com/";
			HttpContext.Current = new HttpContext(new HttpRequest("", tstUrl, ""), new HttpResponse(new StringWriter()))
				{
					User = new GenericPrincipal(new GenericIdentity(userInfo.ReferenceId), new string[0])
				};

			externalContentMapperType = typeof(Bfw.PX.PXPub.Controllers.Mappers.ExternalContentMapper);
            _externalDomainController = new ExternalDomainController();

		}

        [TestCategory("ExternalContentMapper"), TestMethod]
		public void GetAppRoot_WhenDomainIsNull_ReturnsDefaultRoot()
		{
			_context.Domain.Returns((Domain) null);
			MethodInfo m = externalContentMapperType.GetMethod("GetAppRoot", BindingFlags.Static| BindingFlags.NonPublic);

            var objParams = new object[] { _context };

			var obj = m.Invoke(externalContentMapperType, objParams);
			string appRoot = obj.ToString();

			Assert.AreNotEqual(appRoot, "http://lcl.worthpublishers.com/brainhoney");

		}

        [TestCategory("ExternalContentMapper"), TestMethod]
		public void GetAppRoot_WhenDomainIsValid_ReturnsUrlRoot()
		{
			MethodInfo m = externalContentMapperType.GetMethod("GetAppRoot", BindingFlags.Static | BindingFlags.NonPublic);

            var objParams = new object[] { _context };

			var actualResult = m.Invoke(externalContentMapperType, objParams);

			object expectedResult = "http://lcl.worthpublishers.com/brainhoney";

			Assert.AreEqual(actualResult, expectedResult);	

		}

        /// <summary>
        /// If arga is not defined in web config, then expect to return default url
        /// </summary>
        [TestCategory("ExternalContentMapper"), TestMethod]
        public void GetArgaUrl_IfNotDefinedInConfig_ReturnDefaultArgaUrl()
        {
            ConfigurationManager.AppSettings["arga"] = null;
            MethodInfo m = externalContentMapperType.GetMethod("GetArgaUrl", BindingFlags.Static | BindingFlags.NonPublic);

            var objParams = new object[] { _context };
            _context.URL = "http://qa.worthpublishers.com/launchpad/myers10e/182906/Home#/launchpad/item/MODULE_bsi__AF576055__2AED__47CB__8456__31112A4946B3";
            var actualResult = m.Invoke(externalContentMapperType, objParams);

            Assert.AreEqual("/BFWglobal/js/ARGA/ARGA_wrapper.js", actualResult);

        }

        /// <summary>
        /// If arga is defined in web config, then expect to return url from web config with proper domain
        /// </summary>
        [TestCategory("ExternalContentMapper"), TestMethod]
        public void GetArgaUrl_IfDefinedInConfig_ReturnArgaUrl()
        {
            MethodInfo m = externalContentMapperType.GetMethod("GetArgaUrl", BindingFlags.Static | BindingFlags.NonPublic);
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            var objParams = new object[] { _context };
            _context.URL = "http://qa.worthpublishers.com/launchpad/myers10e/182906/Home#/launchpad/item/MODULE_bsi__AF576055__2AED__47CB__8456__31112A4946B3";
            var actualResult = m.Invoke(externalContentMapperType, objParams);

            Assert.AreEqual("http://qa.worthpublishers.com/BFWglobal/js/ARGA/ARGA_wrapper.js", actualResult);

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ExternalDomainMappingIsTrue_SetDefaultLearningCurveUrlToPXUrl()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://learningcurve.bfwpub.com/index.php", Sco = true };
            _externalDomainController.ChangeSettings(new Models.ExternalDomainMappingConfig { Enable = true });
            _externalDomainController.AddMappings("http://learningcurve.bfwpub.com/index.php",
                "/PxLearningCurve/Player/Play/index.php");
            var externalContent = biz.ToExternalContent(_context);
            Assert.IsTrue(externalContent.Url.Contains("/externalcontent/dev-learningcurve.bfwpub.com/index.php"));

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ExternalDomainMappingIsFalse_SetDefaultLearningCurveUrlToUrlDefinedInWebConfig()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://learningcurve.bfwpub.com/index.php",Sco = true };
            _externalDomainController.ChangeSettings(new Models.ExternalDomainMappingConfig { Enable = false });

            var externalContent = biz.ToExternalContent(_context);
            Assert.IsTrue(externalContent.Url.Contains("http://dev-learningcurve.bfwpub.com/index.php"));

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_NoChangeIfThisIsNotLearningCurveUrl()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            _externalDomainController.ChangeSettings(new Models.ExternalDomainMappingConfig() { Enable = true });
            _externalDomainController.AddMappings("http://learningcurve.bfwpub.com/index.php",
                "/PxLearningCurve/Player/Play/index.php");
            var externalContent = biz.ToExternalContent(_context);
            Assert.IsTrue(externalContent.Url.Contains("http://something-else.com"));

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_IfItemHasDueDate_ExpectDueDateIsPassedToModel()
        {
            ContentItem biz = new ContentItem { Href = "http://something-else.com", AssignmentSettings = new AssignmentSettings { DueDate = new DateTime(2014, 1, 1) } };
            var externalContent = biz.ToExternalContent(_context);
            Assert.AreEqual(biz.AssignmentSettings.DueDate, externalContent.DueDate);

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ArgaItem_IfItemHasDueDate_ExpectDueDateInUrlQuery()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";

            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true, AssignmentSettings = new AssignmentSettings { DueDate = new DateTime(2014, 1, 1,0, 0, 0) } };
            _context.Course = new Course { CourseTimeZone = "Eastern Standard Time" };
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.Url.Contains("dueDate=1388552400"));

        }
        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ArgaItem_IfItemNoDueDate_ExpectDueDateIsZeroUrlQuery()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.Url.Contains("dueDate=0"));

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ForScoItem_TrackMinutesSpentShouldBeTrue()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.TrackMinutesSpent);

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ForNonScoItem_TrackMinutesSpentShouldBeTrue()
        {
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = false };
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.TrackMinutesSpent);

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ForLearningCurveItem_TrackMinutesSpentShouldBeFalse()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            biz.FacetMetadata.Add("meta-content-type", "student_LearningCurve");
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsFalse(externalContent.TrackMinutesSpent);

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ArgaItem_IfTrackMinutesSpent_ExpectUrlContainsTrackFalse()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.Url.Contains("track=false"));

        }


        [TestCategory("ExternalContentMapper"), TestMethod]
        public void ToExternalContent_ArgaItem_IfTrackMinutesSpent_ExpectUrlContainsTrackTrue()
        {
            ConfigurationManager.AppSettings["arga"] = "http://[domain]/BFWglobal/js/ARGA/ARGA_wrapper.js";
            ContentItem biz = new ContentItem { Href = "http://something-else.com", Sco = true };
            biz.FacetMetadata.Add("meta-content-type", "student_LearningCurve");

            var externalContent = biz.ToExternalContent(_context);

            Assert.IsTrue(externalContent.Url.Contains("track=true"));

        }

        [TestCategory("ExternalContentMapper"), TestMethod]
        [ExpectedException(typeof(MappingDataMissingException))]
        public void ToExternalContent_WithEmptyUrl_ThrowsException()
        {
            ContentItem biz = new ContentItem ();
            biz.ToExternalContent(_context);
        }
	}
}
