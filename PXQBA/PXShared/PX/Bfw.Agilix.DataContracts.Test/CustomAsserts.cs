using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.DataContracts.Test
{
    public static class CustomAsserts
    {
        public static String FormatMessage(String objectName, String propertyName, String message)
        {
            if (String.IsNullOrEmpty(message))
                return String.Format("{0}.{1}", objectName, propertyName);

            return String.Format("{0}.{1} -- {2}", objectName, propertyName, message);
        }

        public static void Assert_AreEqual(this AgilixUser expected, AgilixUser actual, String message)
        {
            Assert.AreEqual(expected.Email, actual.Email, FormatMessage("AgilixUser", "Email", message));
            Assert.AreEqual(expected.FirstName, actual.FirstName, FormatMessage("AgilixUser", "FirstName", message));
            Assert.AreEqual(expected.Id, actual.Id, FormatMessage("AgilixUser", "Id", message));
            Assert.AreEqual(expected.LastLogin, actual.LastLogin, FormatMessage("AgilixUser", "LastLogin", message));
            Assert.AreEqual(expected.LastName, actual.LastName, FormatMessage("AgilixUser", "LastName", message));
            Assert.AreEqual(expected.Reference, actual.Reference, FormatMessage("AgilixUser", "Reference", message));

            expected.Credentials.Assert_AreEqual(actual.Credentials, message);
            expected.Domain.Assert_AreEqual(actual.Domain, message);
            expected.GlobalId.Assert_AreEqual(actual.GlobalId, FormatMessage("AgilixUser", "Reference", message));
        }

        public static void Assert_AreEqual(this Domain expected, Domain actual, String message)
        {
            Assert.AreEqual(expected.Id, expected.Id,FormatMessage("Domain", "Id", message));
            Assert.AreEqual(expected.Name, expected.Name, FormatMessage("Domain", "Name", message));
            Assert.AreEqual(expected.Reference, expected.Reference, FormatMessage("Domain", "Reference", message));
            Assert.AreEqual(expected.Userspace, expected.Userspace, FormatMessage("Domain", "Userspace", message));
        }

        public static void Assert_AreEqual(this Credentials expected, Credentials actual, String message)
        {
            Assert.AreEqual(expected.Password, actual.Password, FormatMessage("Credentials", "Password", message));
            Assert.AreEqual(expected.PasswordAnswer, actual.PasswordAnswer, FormatMessage("Credentials", "PasswordAnswer", message));
            Assert.AreEqual(expected.PasswordQuestion, actual.PasswordQuestion, FormatMessage("Credentials", "PasswordQuestion", message));
            Assert.AreEqual(expected.Username, actual.Username, FormatMessage("Credentials", "UserName", message));
            Assert.AreEqual(expected.UserSpace, actual.UserSpace, FormatMessage("Credentials", "UserSpace", message));
        }

        public static void Assert_AreEqual(this Guid? expected, Guid? actual, String message)
        {
            Assert.AreEqual(expected, actual, message);
        }


        /// <summary>
        /// comparing an ItemActivity object using Assert statements. 
        /// Makes the code readable and less cluttered
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="index">Activities are returned in an array format, this is the index of that array for logging purposes only</param>
        public static void Assert_AreEqual(this ItemActivity expected, ItemActivity actual, String message)
        {
            Assert.AreEqual(expected.EnrollmentId, actual.EnrollmentId, String.Format("EnrollmentId[{0}]", message));
            Assert.AreEqual(expected.ItemId, actual.ItemId, String.Format("ItemId[{0}]", message));
            Assert.AreEqual(expected.Seconds, actual.Seconds, String.Format("Seconds[{0}]", message));
            Assert.AreEqual(expected.StartTime, actual.StartTime, String.Format("StartTime[{0}]", message));
        }

        public static void Assert_AreEqual(this Announcement expected, Announcement actual, String message)
        {
            Assert.AreEqual(expected.CreationDate, actual.CreationDate, FormatMessage("Announcement", "CreationDate", message));
            Assert.AreEqual(expected.EndDate, actual.EndDate, FormatMessage("Announcement", "EndDate", message));
            Assert.AreEqual(expected.EntityId, actual.EntityId, FormatMessage("Announcement", "EntityId", message));
            Assert.AreEqual(expected.Html, actual.Html, FormatMessage("Announcement", "Html", message));
            Assert.AreEqual(expected.Path, actual.Path, FormatMessage("Announcement", "Path", message));
            Assert.AreEqual(expected.PinSortOrder, actual.PinSortOrder, FormatMessage("Announcement", "PinSortOrder", message));
            Assert.AreEqual(expected.PrimarySortOrder, actual.PrimarySortOrder, FormatMessage("Announcement", "PrimarySortOrder", message));
            Assert.AreEqual(expected.StartDate, actual.StartDate, FormatMessage("Announcement", "StartDate", message));
            Assert.AreEqual(expected.Title, actual.Title, FormatMessage("Announcement", "Title", message));
            Assert.AreEqual(expected.Version, actual.Version, FormatMessage("Announcement", "Version", message));
        }

        public static void Assert_AreEqual(this TeacherResponse expected, TeacherResponse actual, String message)
        {
            Assert.AreEqual(expected.ForeignId, actual.ForeignId, FormatMessage("TeacherResponse", "ForeignId", message));
            Assert.AreEqual(expected.Mask, actual.Mask, FormatMessage("TeacherResponse", "Mask", message));
            Assert.AreEqual(expected.PointsAssigned, actual.PointsAssigned, FormatMessage("TeacherResponse", "PointsAssigned", message));
            Assert.AreEqual(expected.PointsComputed, actual.PointsComputed, FormatMessage("TeacherResponse", "PointsComputed", message));
            Assert.AreEqual(expected.PointsPossible, actual.PointsPossible, FormatMessage("TeacherResponse", "PointsPossible", message));

            Assert.AreEqual(expected.ScoredVersion, actual.ScoredVersion, FormatMessage("TeacherResponse", "ScoredVersion", message));
            Assert.AreEqual(expected.Status, actual.Status, FormatMessage("TeacherResponse", "Status", message));
            Assert.AreEqual(expected.StudentEnrollmentId, actual.StudentEnrollmentId, FormatMessage("TeacherResponse", "StudentEnrollmentId", message));
            Assert.AreEqual(expected.SubmittedDate, actual.SubmittedDate, FormatMessage("TeacherResponse", "SubmittedDate", message));
            Assert.AreEqual(expected.TeacherResponseType, actual.TeacherResponseType, FormatMessage("TeacherResponse", "TeacherResponseType", message));

            Assert.IsTrue((expected.Responses == null && actual.Responses == null) || (expected.Responses != null && actual.Responses != null), FormatMessage("TeacherResponse", "Responses", message));
            Assert.IsTrue(expected.Responses != null && actual.Responses != null, FormatMessage("TeacherResponse", "Responses", message));
            if (null != expected.Responses)
            {
                Assert.AreEqual(expected.Responses.Count, actual.Responses.Count, FormatMessage("TeacherResponse", "Responses", message));
                for(int i = 0 ; i < expected.Responses.Count; i++)
                    Assert_AreEqual(expected.Responses[i], actual.Responses[i], FormatMessage("TeacherResponse", String.Format("Responses[{0}]", i), message));
            }
        }
    }
}
