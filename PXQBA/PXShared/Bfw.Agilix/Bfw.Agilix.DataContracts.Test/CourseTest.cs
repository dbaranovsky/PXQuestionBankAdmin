using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class CourseTest
    {
        //ParseEntity Test
        [TestMethod, TestCategory("Course")]
        public void CourseTest_ToEntity_CheckIf_LMSId_Required_Assigned()
        {
            var oCourse = new XElement("Course");
            var oData = new XElement("data");
            var lmsidrequired = new XElement("bfw_lmsid_required", true);
            var properties = new XElement("bfw_properties");
            oData.Add(lmsidrequired);
            oData.Add(properties);
            oCourse.Add(oData);

            var course = new Course() { Id = "9959", Title = "Test Course", Data=oData };

            course.ToEntity();
            var idrequired = bool.Parse(course.Data.Element("bfw_lmsid_required").Value.ToString());
            Assert.AreEqual(true, idrequired);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_ToEntity_CheckIf_LMSId_Label_Assigned()
        {
            var oCourse = new XElement("Course");
            var oData = new XElement("data");
            var lmsidrequired = new XElement("bfw_lmsid_label", "LMS Id");
            var properties = new XElement("bfw_properties");
            oData.Add(lmsidrequired);
            oData.Add(properties);
            oCourse.Add(oData);

            var course = new Course() { Id = "9959", Title = "Test Course", Data = oData };

            course.ToEntity();
            var idlabel = course.Data.Element("bfw_lmsid_label").Value.ToString();
            Assert.AreEqual("LMS Id", idlabel);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_ParseEntity_CheckIf_LMSId_Prompt_Assigned()
        {
            var oCourse = new XElement("Course");
            var oData = new XElement("data");
            var lmsidrequired = new XElement("bfw_lmsid_prompt", "Please enter LMS Id");
            var properties = new XElement("bfw_properties");
            oData.Add(lmsidrequired);
            oData.Add(properties);
            oCourse.Add(oData);

            var course = new Course() { Id = "9959", Title = "Test Course", Data = oData };

            course.ToEntity();
            var idprompt = course.Data.Element("bfw_lmsid_prompt").Value.ToString();

            Assert.AreEqual("Please enter LMS Id", idprompt);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_ParseEntity_ShouldaParse_DisableComments()
        {
            var value = "true";
            var oCourse = new XElement("Course");
            var oData = new XElement("data");
            var disableComments = new XElement("bfw_disable_comments", value);
            oData.Add(disableComments);
            oCourse.Add(oData);

            Course c = new Course();
            c.ParseEntity(oCourse);
            var parsedval = c.Data.Element("bfw_disable_comments").Value.ToString();

            Assert.AreEqual(value, parsedval);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_ToEntity_Should_Add_Generic_CourseId_If_Missing()
        {
            var course = new Course();
            var xmlResponse = Helper.GetResponse(Entity.Course, "GenericCourse");
            var xmlCourse = xmlResponse.Element("course");
            xmlCourse.Element("data").Add(new XElement("bfw_generic_course"));
            course.ParseEntity(xmlCourse);
            course.GenericCourseId = "genericCourseId";

            var result = course.ToEntity();

            Assert.AreEqual("genericCourseId", result.Element("data").Element("bfw_generic_course").Attribute("id").Value);
        }

        [TestMethod, TestCategory("Course")]
        public void CourseTest_ToEntity_Should_Not_Add_Generic_CourseId_If_Present()
        {
            var course = new Course();
            var xmlResponse = Helper.GetResponse(Entity.Course, "GenericCourse");
            var xmlCourse = xmlResponse.Element("course");
            xmlCourse.Element("data").Add(new XElement("bfw_generic_course"));
            xmlCourse.Element("data").Element("bfw_generic_course").Add(new XAttribute("id", "oldGenericCourseId"));
            course.ParseEntity(xmlCourse);
            course.GenericCourseId = "genericCourseId";

            var result = course.ToEntity();

            Assert.AreEqual("genericCourseId", result.Element("data").Element("bfw_generic_course").Attribute("id").Value);
        }
    }
}
