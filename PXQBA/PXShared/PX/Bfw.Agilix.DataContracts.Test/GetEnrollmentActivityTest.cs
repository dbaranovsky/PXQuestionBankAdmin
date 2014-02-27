using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts.Test;
using System.IO;

namespace Bfw.Agilix.DataContracts.Test
{
    [TestClass]
    public class GetEnrollmentActivityTest : BaseTest
    {
        #region Before and After each Test methods

        private const String 
            path = @"GetEnrollmentActivityTest\",
            outputDir = "GetEnrollmentActivityTest";
        
        [TestInitialize]
        public void BeforeEachTest()
        {
        }

        [TestCleanup]
        public void AfterEachTest()
        {
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Testing to see if the Parsing of the response Object is working for a failure to find any Activities.
        /// </summary>
        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void SearchFoundZeroActivitiesTest()
        {
            var cmd = new GetEnrollmentActivity() { SearchParameter = new EnrollmentActivitySearch { EnrollmentId = "5" } };

            ProcessCommand(cmd, getFileLocator("NoEnrollmentActivity"));
            var activities = cmd.Activity.ToList();

            Assert.IsNotNull(activities, "Activties");
            Assert.AreEqual(0, activities.Count, "Number of activities in result");
        }

        /// <summary>
        /// Testing to see if the Parsing of the response Object is working for multiple Activities.
        /// In this case 2 Activities
        /// </summary>
        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void SearchFoundTwoActivitiesTest()
        {
            try
            {
                var cmd = new GetEnrollmentActivity() { SearchParameter = new EnrollmentActivitySearch { EnrollmentId = "5" } };

                ProcessCommand(cmd, getFileLocator("EnrollmentActivity"));
                var activities = cmd.Activity.ToList();

                Assert.IsNotNull(activities, "Activties");
                Assert.AreEqual(2, activities.Count, "Number of activities in result");

                ItemActivity expectedActivity1 = new ItemActivity()
                {
                    EnrollmentId = "5",
                    ItemId = "C1",
                    Seconds = 154,
                    StartTime = DateTime.Parse("2011-08-18T16:06:51.747Z")
                };
                ItemActivity expectedActivity2 = new ItemActivity()
                {
                    EnrollmentId = "5",
                    ItemId = "E1",
                    Seconds = 20,
                    StartTime = DateTime.Parse("2011-10-17T21:17:42.743Z")
                };
                
                expectedActivity1.Assert_AreEqual(activities[0], "0");
                expectedActivity2.Assert_AreEqual(activities[1], "1");
            }
            catch (Exception e)
            {
                Assert.Fail(String.Format("Unexpected Exception ({0} - {1})", e.GetType().Name, e.Message));
            }
        }

        #endregion
    }
}
