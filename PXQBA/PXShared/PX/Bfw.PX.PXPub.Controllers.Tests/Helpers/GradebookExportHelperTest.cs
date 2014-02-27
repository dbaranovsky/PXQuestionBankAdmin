using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Controllers.Tests.Helpers
{
    [TestClass]
    public class GradebookExportHelperTest
    {
        IGradebookExportHelper helper;

        [TestInitialize]
        public void TestInitialize()
        {
            helper = new GradebookExportHelper();
        }

        [TestMethod]
        public void GetCsvString_Should_Generate_CSV_String()
        {
            var category = new CategoryGrade()
            {
                Id = "1",
                Name = "Default"
            };
            var user = new UserInfo() 
            { 
                FirstName = "firstName",
                LastName = "lastName",
                ReferenceId = "referenceId",
                Email = "my@email.com"
            };
            var enrollment = new Enrollment()
            {
                CategoryGrades = new List<CategoryGrade>() 
                { 
                    category                    
                },
                User = user,
                OverallAchieved = 5,
                OverallPossible = 10,
                OverallGrade = "50%"
            };
            var enrollments = new List<Enrollment>();
            enrollments.Add(enrollment);

            var result = helper.GetCsvString(enrollments);

            Assert.AreEqual("FullName,LMS ID,Email,Points Achieved,Points Possible,Score,Default Points Achieved,Default Points Possible\r\nfirstName lastName,referenceId,my@email.com,5,10,50%,0,0", result);
        }
    }
}
