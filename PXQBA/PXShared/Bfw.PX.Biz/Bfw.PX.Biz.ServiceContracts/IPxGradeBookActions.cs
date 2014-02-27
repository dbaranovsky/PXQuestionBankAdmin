using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts to peform GradeBook actions  
    /// </summary>
    public interface IPxGradeBookActions
    {
        /// <summary>
        /// Gets the list of students for a gradebook
        /// </summary>
        /// <returns></returns>
        IEnumerable<StudentProfile> GetStudentList();

        /// <summary>
        /// Get Student Profile
        /// </summary>
        /// <param name="studentUserId"></param>
        /// <returns></returns>
        StudentProfile GetStudent(string studentUserId);

        /// <summary>
        /// Returns the list of all grade book assignments in a course
        /// </summary>
        /// <returns></returns>
        IDictionary<ContentItem, IList<ContentItem>> GetGradeBookAssignments();

        /// <summary>
        /// Gets list of enrollments for all <paramref name="itemIds"/>
        /// </summary>
        /// <param name="itemIds">List of item id's to return enrollments for.  If null returns enrollments for all items
        /// associated with the active entity</param>
        /// <returns>List of enrollments associated with the id's of the items passed in</returns>
        IEnumerable<Enrollment> GetEnrollments(IEnumerable<String> itemIds = null);

        /// <summary>
        /// Get the Grades of a student given the student enrollment id
        /// </summary>
        /// <param name="enrollmentid">The enrollment id of the student</param>
        /// <param name="assigned">True for assigned grades. False for unassigned</param>
        /// <returns></returns>
        IEnumerable<Grade> GetGradesByEnrollment(string enrollmentid, bool assigned);


        /// <summary>
        /// Returns a collection of assigned or unassiced grades aggregated by student
        /// </summary>
        /// <param name="assigned">If true returns assigned grades, else unassigned</param>
        /// <returns>Collection of assigned or unassigned grades aggregated by student</returns>
        IEnumerable<DataContracts.Grade> StudentAggregatedGrades(bool assigned);
        /// <summary>
        /// Returns collection of student grades associated with <paramref name="itemiD" />
        /// </summary>
        /// <param name="itemId">Id to retreive student grades for</param>
        /// <param name="assigned">If the item passed in is an assigned item</param>
        /// <returns>Collection of grade details per student for <paramref name="itemId"/></returns>
        IEnumerable<DataContracts.Grade> StudentGradeDetails(string itemId, bool assigned);

        /// <summary>
        /// Gets a collection of grades aggregated by content item
        /// </summary>
        /// <param name="assigned">If true returns assigned items.  Else returns unassigned items</param>
        /// <returns>Collection of aggregated assigned or unassigned items</returns>
        IEnumerable<DataContracts.Grade> ItemAggregatedGrades(bool assigned);
        /// <summary>
        /// Gets collections of grades (either assigned or unassigned) for a <paramref name="enrollmentId"/>
        /// </summary>
        /// <param name="enrollmentId">EnrollmentId to return grades for</param>
        /// <param name="assigned">If true returns assigned items.  Else unassigned</param>
        /// <returns>Collection of grades for <paramref name="enrollmentId"/></returns>
        IEnumerable<DataContracts.Grade> ItemGradeDetails(string enrollmentId, bool assigned);

        /// <summary>
        ///  Get student attempts by assignmentid and enrollmentid
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="EnrollmentId"></param>
        /// <returns></returns>
        IEnumerable<SubmissionLog> GetAttemptsByStudent(string ItemId, string EnrollmentId);
    }
}
