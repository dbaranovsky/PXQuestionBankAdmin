using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers.Tests.Mappers
{
    [TestClass]
    public class ActivitiesWidgetMapperTest
    {
        /// <summary>
        /// Make sure the activities return from SortByDueDate() are in the right order
        /// Assigned activites must be ordered before Unassigned activities
        /// Assigned activities must be sorted by their due dates.
        /// </summary>
        [TestCategory("ActivitiesWidgetMapper"), TestMethod]
        public void SortByDueDate_ExpectAllActivitiesAreSortedByDueDate()
        {
            var activities = GenerateRandomActivities(1000);
            activities = activities.SortByDueDate();
            var isInOrder = true;
            for (int i = 1; (i < activities.Count - 1) && isInOrder; i++)
            {
                var previousActivity = activities[i - 1];
                var currentActivity = activities[i];
                var nextActivity = activities[i + 1];
                //If this activity is assigned
                if (currentActivity.isAssigned)
                {
                    //Compare this activity to the previous one
                    //Due date of this activity must be greater than previous.
                    if (previousActivity.DueDate > currentActivity.DueDate)
                        isInOrder = false;

                    //Compare this activity to the next one
                    //If next activty is assigned, then its due date must be equal or greater than this one
                    if (nextActivity.isAssigned && currentActivity.DueDate > nextActivity.DueDate)
                        isInOrder = false;
                }
                else
                {
                    //If this activity is unassigned, then the next one must also be unassigned.
                    if (nextActivity.isAssigned)
                        isInOrder = false;

                }

            }
            Assert.IsTrue(isInOrder);

        }

        #region Implementation

        private List<Activity> GenerateRandomActivities(int number)
        {
            var activites = new List<Activity>();
            Random randomGenerator = new Random();
            for (int i = 0; i < number; i ++)
            {
                int randomNumber = randomGenerator.Next((int)(DateTime.MaxValue - DateTime.MinValue).TotalDays);

                bool isAssigned = randomNumber%2 == 1;

                var activity = new Activity { isAssigned = isAssigned ,
                                              DueDate = isAssigned? DateTime.MinValue.AddDays(randomNumber):DateTime.MinValue
                };
                activites.Add(activity);
            }
            return activites;

        }
        #endregion Implementation
    }
}
