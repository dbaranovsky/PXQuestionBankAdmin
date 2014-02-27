using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class AgilixUserTest
    {
        [TestMethod]
        public void Constructor_Should_Initialize_GradebookSettingsFlagsCourse()
        {
            var user = new AgilixUser();

            Assert.IsNotNull(user.GradebookSettingsFlagsCourse);
        }

        [TestMethod]
        public void GradebookSettingsFlagsCourse_Should_Set_And_Get()
        {
            var user = new AgilixUser();
            user.GradebookSettingsFlagsCourse.Add("testFlag", "testValue");

            Assert.AreEqual("testFlag", user.GradebookSettingsFlagsCourse.FirstOrDefault().Key);
            Assert.AreEqual("testValue", user.GradebookSettingsFlagsCourse.FirstOrDefault().Value);
        }

        [TestMethod]
        public void ToEntity_Should_Add_GradebookSettingsFlagsCourse_Element()
        {
            var user = new AgilixUser();
            
            user.GradebookSettingsFlagsCourse.Add("testFlag", "testValue");

            var element = user.ToEntity();

            Assert.IsTrue(element.ToString().Contains("<pref_gradebook_testFlag-gradeview>testValue</pref_gradebook_testFlag-gradeview>"));
        }

        [TestMethod]
        public void ParseEntity_Should_Parse_GradebookSettingsFlagsCourse_Element()
        {
            var user = new AgilixUser();
            var settings = new XElement("pref_gradebook_testFlag-gradeview", "testValue");
            var xdata = new XElement("data");
            xdata.Add(settings);
            var xuser = new XElement("user");
            xuser.Add(xdata);

            user.ParseEntity(xuser);

            Assert.AreEqual("testFlag", user.GradebookSettingsFlagsCourse.FirstOrDefault().Key);
            Assert.AreEqual("testValue", user.GradebookSettingsFlagsCourse.FirstOrDefault().Value);
        }
    }
}
