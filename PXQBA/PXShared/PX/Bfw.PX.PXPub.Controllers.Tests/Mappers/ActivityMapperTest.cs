using System.Configuration;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.ContentTypes;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    
    
    /// <summary>
    ///This is a test class for ActivityMapper and is intended
    ///to contain all ActivityMapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class ActivityMapperTest
    {
        private IBusinessContext _context;
        private IContentActions _contActions;
        private ExternalDomainController _argaController;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _contActions = Substitute.For<IContentActions>();
            _context = Substitute.For<IBusinessContext>();
            _context.Course = new Bfw.PX.Biz.DataContracts.Course { EnableArgaUrlMapping = true };
            _argaController = new ExternalDomainController();
        }

        [TestCategory("ActivityMapper"), TestMethod]
        public void ExternalContent_SetDefaultLearningCurveUrlToUrlDefinedInWebConfig()
        {
            var biz = new Bfw.PX.Biz.DataContracts.ContentItem { Type = "", Href = "http://learningcurve.bfwpub.com/index.php" };
            var activity = biz.ToActivity(_context, _contActions);
            Assert.AreEqual("/externalcontent/dev-learningcurve.bfwpub.com/index.php", activity.Href);

        }

        [TestCategory("ActivityMapper"), TestMethod]
        public void ToExternalContent_NoChangeIfThisIsNotLearningCurveUrl()
        {
            var biz = new Bfw.PX.Biz.DataContracts.ContentItem { Type = "", Href = "http://something-else.com" };
            ConfigurationManager.AppSettings["learningCurveDomain"] = "http://learningcurve.bfwpub.com";
            var activity = biz.ToActivity(_context, _contActions);
            Assert.AreEqual(activity.Href, "http://something-else.com");

        }

        [TestCategory("ActivityMapper"), TestMethod]
        public void ToExternalContent_NoChangeIfThisUrlIsNull()
        {
            var biz = new Bfw.PX.Biz.DataContracts.ContentItem { Type = "" };
            ConfigurationManager.AppSettings["learningCurveDomain"] = "http://dev-learningcurve.bfwpub.com";
            var activity = biz.ToActivity(_context, _contActions);
            Assert.IsTrue(string.IsNullOrEmpty(activity.Href));

        }

        
    }
}
