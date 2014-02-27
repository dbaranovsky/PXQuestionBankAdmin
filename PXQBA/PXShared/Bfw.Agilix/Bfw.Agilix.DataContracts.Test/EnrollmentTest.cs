using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class EnrollmentTest
    {
        [TestMethod]
        public void ParseEntity_Should_Parse_User_Email_And_Reference()
        {
            Enrollment enrollment = new Enrollment();
            var user = new XElement("user");
            user.Add(new XAttribute("reference", "8472925"));
            user.Add(new XAttribute("email", "studenta@paluckiuniversity.edu"));
            var entity = new XElement("enrollment");
            entity.Add(user);

            enrollment.ParseEntity(entity);

            Assert.AreEqual(enrollment.User.Reference, "8472925");
            Assert.AreEqual(enrollment.User.Email, "studenta@paluckiuniversity.edu");
        }

        [TestMethod]
        public void ParseEntity_Should_Parse_Gradebook_Categories()
        {
            Enrollment enrollment = new Enrollment();
            var category = new XElement("category");
            category.Add(new XAttribute("id", "3"));
            category.Add(new XAttribute("name", "Clicker questions"));
            category.Add(new XAttribute("achieved", "9"));
            category.Add(new XAttribute("possible", "15"));
            category.Add(new XAttribute("letter", "D"));
            var categories = new XElement("categories");
            categories.Add(category);
            var grades = new XElement("grades");
            grades.Add(categories);
            var entity = new XElement("enrollment");
            entity.Add(grades);

            enrollment.ParseEntity(entity);

            Assert.AreEqual("3", enrollment.CategoryGrades.First().Id);
            Assert.AreEqual("Clicker questions", enrollment.CategoryGrades.First().Name);
            Assert.AreEqual(9, enrollment.CategoryGrades.First().Achieved);
            Assert.AreEqual(15, enrollment.CategoryGrades.First().Possible);
            Assert.AreEqual("D", enrollment.CategoryGrades.First().Letter);
        }

        [TestMethod]
        public void ParseEntity_Should_Parse_Priviliges()
        {
            Enrollment enrollment = new Enrollment();
            var entity = new XElement("enrollment");
            entity.Add(new XAttribute("privileges", "123456"));

            enrollment.ParseEntity(entity);

            Assert.AreEqual("123456", enrollment.Priviliges);
        }
    }
}
