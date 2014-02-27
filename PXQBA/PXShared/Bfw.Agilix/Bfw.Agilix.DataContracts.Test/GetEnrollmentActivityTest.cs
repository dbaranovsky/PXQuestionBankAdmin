using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Test
{
    [TestClass]
    public class GetEnrollmentActivityTest : BaseTest
    {
        #region Before and After each Test methods

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
        public void SearchFoundZeroActivitiesTest()
        {
            try
            {
                var cmd = new GetEnrollmentActivity() { SearchParameter = new EnrollmentActivitySearch { EnrollmentId = "5" } };

                ProcessCommand(cmd, getFileLocator("NoEnrollmentActivity"));
                var activities = cmd.Activity.ToList();

                Assert.IsNotNull(activities, "Activties");
                Assert.AreEqual(0, activities.Count, "Number of activities in result");
            }
            catch (Exception e)
            {
                Assert.Fail(String.Format("Unexpected Exception ({0} - {1})", e.GetType().Name, e.Message));
            }
        }

        /// <summary>
        /// Testing to see if the Parsing of the response Object is working for multiple Activities.
        /// In this case 2 Activities
        /// </summary>
        [TestMethod]
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
                AssertItemActivity(expectedActivity1, activities[0], 0);
                AssertItemActivity(expectedActivity2, activities[1], 1);
            }
            catch (Exception e)
            {
                Assert.Fail(String.Format("Unexpected Exception ({0} - {1})", e.GetType().Name, e.Message));
            }
        }

        #endregion

        #region Assert Helpers

        /// <summary>
        /// comparing an ItemActivity object using Assert statements. 
        /// Makes the code readable and less cluttered
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="index">Activities are returned in an array format, this is the index of that array for logging purposes only</param>
        private void AssertItemActivity(ItemActivity expected, ItemActivity actual, int index = 0)
        {
            Assert.AreEqual(expected.EnrollmentId, actual.EnrollmentId, String.Format("EnrollmentId[{0}]", index));
            Assert.AreEqual(expected.ItemId, actual.ItemId, String.Format("ItemId[{0}]", index));
            Assert.AreEqual(expected.Seconds, actual.Seconds, String.Format("Seconds[{0}]", index));
            Assert.AreEqual(expected.StartTime, actual.StartTime, String.Format("StartTime[{0}]", index));
        }

        #endregion
    }
}
