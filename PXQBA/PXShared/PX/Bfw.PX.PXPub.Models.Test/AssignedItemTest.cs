using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.PXPub.Models.Test
{
    [TestClass]
    public class AssignedItemTest
    {
        /// <summary>
        /// If item is Sco item and content type is learningCurve, "Use Score Earned" should not be available.
        /// </summary>
        [TestCategory("AssignedItem"), TestMethod]
        public void GetAvailableSubmissionGradeAction_WhenContentTypeIsLearningCurve_ExpectUseScoreEarnedNotAvailable()
        {
            var assignedItem = new AssignedItem {Sco = true, ContentType = "LearningCurve"};
            var availableSubmissionGradeActions = assignedItem.GetAvailableSubmissionGradeAction();
            Assert.IsFalse(availableSubmissionGradeActions.Any(action => action.Key == (Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Default))));
        }

        /// <summary>
        /// If item is Sco item and content type is null, "Use Score Earned" should be available.
        /// </summary>
        [TestCategory("AssignedItem"), TestMethod]
        public void GetAvailableSubmissionGradeAction_WhenContentTypeIsLearningCurve_ExpectUseScoreEarnedAvailable()
        {
            var assignedItem = new AssignedItem { Sco = true, ContentType = null};
            var availableSubmissionGradeActions = assignedItem.GetAvailableSubmissionGradeAction();
            Assert.IsTrue(availableSubmissionGradeActions.Any(action => action.Key == (Enum.GetName(typeof(SubmissionGradeAction), SubmissionGradeAction.Default))));
        }
    }
}
