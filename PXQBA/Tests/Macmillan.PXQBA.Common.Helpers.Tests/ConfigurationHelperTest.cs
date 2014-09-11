using System.Configuration;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Common.Helpers.Tests
{

    [TestClass]
    public class ConfigurationHelperTest
    {

        [TestMethod]
        public void GetMarsLoginUrl_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.MarsPathLoginUrl] = expectedSetting;

            var result = ConfigurationHelper.GetMarsLoginUrl();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetMarsLogoutUrl_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.MarsPathLogoutUrl] = expectedSetting;

            var result = ConfigurationHelper.GetMarsLogoutUrl();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetBrainhoneyDefaultPassword_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.BrainhoneyDefaultPassword] = expectedSetting;

            var result = ConfigurationHelper.GetBrainhoneyDefaultPassword();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetQuestionPerPage_NoParameters_CorrectData()
        {
            const string expectedSetting = "50";

            ConfigurationManager.AppSettings[ConfigurationKeys.QuestionPerPage] = expectedSetting;

            var result = ConfigurationHelper.GetQuestionPerPage();

            Assert.AreEqual(int.Parse(expectedSetting), result);
        }

        [TestMethod]
        public void GetAdministratorUserspace_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorUserspace] = expectedSetting;

            var result = ConfigurationHelper.GetAdministratorUserspace();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetAdministratorUserId_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorUserId] = expectedSetting;

            var result = ConfigurationHelper.GetAdministratorUserId();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetAdministratorPassword_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorPassword] = expectedSetting;

            var result = ConfigurationHelper.GetAdministratorPassword();

            Assert.AreEqual(expectedSetting, result);
        }

        [TestMethod]
        public void GetDomainId_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.DomainId] = expectedSetting;

            var result = ConfigurationHelper.GetDomainId();

            Assert.AreEqual(expectedSetting, result);
        }


        [TestMethod]
        public void GetDomainUserspace_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.DomainUserspace] = expectedSetting;

            var result = ConfigurationHelper.GetDomainUserspace();

            Assert.AreEqual(expectedSetting, result);
        }


       [TestMethod]
        public void GetDisciplineCourseId_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.DisciplineCourseId] = expectedSetting;

            var result = ConfigurationHelper.GetDisciplineCourseId();

            Assert.AreEqual(expectedSetting, result);
        }


        [TestMethod]
        public void GetActionPlayerUrlTemplate_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.ActionPlayerUrlTemplate] = expectedSetting;

            var result = ConfigurationHelper.GetActionPlayerUrlTemplate();

            Assert.AreEqual(expectedSetting, result);
        }


        [TestMethod]
        public void GetCacheTimeout_NoParameters_CorrectData()
        {
            const string expectedSetting = "20";

            ConfigurationManager.AppSettings[ConfigurationKeys.CacheTimeout] = expectedSetting;

            var result = ConfigurationHelper.GetCacheTimeout();

            Assert.AreEqual(int.Parse(expectedSetting), result);
        }


        [TestMethod]
        public void GetHTSEditorUrlTemplate_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.HTSEditorUrlTemplate] = expectedSetting;

            var result = ConfigurationHelper.GetHTSEditorUrlTemplate();

            Assert.AreEqual(expectedSetting, result);
        }


        [TestMethod]
        public void GetUsersPerPage_NoParameters_CorrectData()
        {
            const string expectedSetting = "50";

            ConfigurationManager.AppSettings[ConfigurationKeys.UsersPerPage] = expectedSetting;

            var result = ConfigurationHelper.GetUsersPerPage();

            Assert.AreEqual(int.Parse(expectedSetting), result);
        }


        [TestMethod]
        public void GetBrainhoneyCourseImageFolder_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.BrainhoneyCourseImageFolder] = expectedSetting;

            var result = ConfigurationHelper.GetBrainhoneyCourseImageFolder();

            Assert.AreEqual(expectedSetting, result);
        }


        [TestMethod]
        public void GetAllowedSubdomains_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.AllowedSubdomains] = expectedSetting;

            var result = ConfigurationHelper.GetAllowedSubdomains();

            Assert.AreEqual(expectedSetting, result);
        }

        
        [TestMethod]
        public void GetDevSubdomain_NoParameters_CorrectData()
        {
            const string expectedSetting = "expected";

            ConfigurationManager.AppSettings[ConfigurationKeys.DevSubdomain] = expectedSetting;

            var result = ConfigurationHelper.GetDevSubdomain();

            Assert.AreEqual(expectedSetting, result);
        }
        
    }
}
