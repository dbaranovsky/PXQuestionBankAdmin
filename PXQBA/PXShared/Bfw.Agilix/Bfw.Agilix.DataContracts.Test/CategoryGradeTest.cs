using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class CategoryGradeTest
    {
        [TestMethod]
        public void ParseEntity_Should_Parse_XElement()
        {
            CategoryGrade grade = new CategoryGrade();
            var entity = new XElement("category");
            entity.Add(new XAttribute("id", "3"));
            entity.Add(new XAttribute("name", "Clicker questions"));
            entity.Add(new XAttribute("achieved", "9"));
            entity.Add(new XAttribute("possible", "15"));
            entity.Add(new XAttribute("letter", "D"));

            grade.ParseEntity(entity);

            Assert.AreEqual("3", grade.Id);
            Assert.AreEqual("Clicker questions", grade.Name);
            Assert.AreEqual(9, grade.Achieved);
            Assert.AreEqual(15, grade.Possible);
            Assert.AreEqual("D", grade.Letter);
        }
    }
}
