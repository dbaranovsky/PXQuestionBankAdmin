using System;
using System.Collections.Generic;
using System.IO;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Determines the status of a graded item.
    /// </summary>
    public enum GradedItemStatus
    {
        Any,
        HasBeenGraded,
        HasBeenSubmittedOrGraded
    }

    /// <summary>
    /// Provides contracts for actions that manipulate grades.
    /// </summary>
    public interface IGradeActions
    {
        /// <summary>
        /// Gets gradebook detail for the specified user.
        /// </summary>
        /// <param name="userId">ID of the user for which to get grades.</param>
        /// <param name="entityId">Optional entity ID by which to filter the returned data.</param>
        /// <param name="status">Graded item status by which to filter the returned data.</param>
        /// <param name="dueAfter">Date range by which to filter the returned data.</param>
        /// <param name="dueBefore">Date range by which to filter the returned data.</param>
        /// <returns></returns>
        IEnumerable<Grade> GetGrades(string userId, string entityId, GradedItemStatus status, DateTime dueAfter, DateTime dueBefore);

        /// <summary>
        /// Gets all assignments/grades with for a student (won't work with impersonated instructor account)
        /// </summary>
        /// <param name="userId">Required User id (takes precedence over enrollment id if not null)</param>
        /// <param name="enrollmentId">Required Enrollment id</param>
        /// <param name="utcOffSet">Required the course time difference between GMT and local time, in minutes.</param>
        /// <param name="showCompleted">Optional flag to show completed assignments (false by default)</param>
        /// <param name="showPastDue">Optional flag to show past due assignments (false by default)(</param>
        /// <param name="days">Optional day range (2 weeks by default)</param>
        /// <returns></returns>
        IEnumerable<Grade> GetDueSoonItemsWithGrades(string userId, string enrollmentId, int utcOffSet, bool showCompleted = false, bool showPastDue = false, int days = 14);

        /// <summary>
        /// Gets all assignments/grades for the enrollment for a student (won't work with impersonated instructor account)
        /// </summary>
        /// <param name="enrollmentId">Required Enrollment id</param>
        /// <param name="utcOffSet">Required the course time difference between GMT and local time, in minutes.</param>
        /// <returns></returns>
        IEnumerable<Grade> GetDueSoonItemsWithGrades(string enrollmentId, int utcOffSet);

        /// <summary>
        /// Get grades for all students for a specific item
        /// </summary>
        /// <param name="entityId">Course that Owns the item</param>
        /// <param name="itemId">List of the Item Ids</param>
        /// <returns>List of grades for all students for this item</returns>
        IEnumerable<DataContracts.Grade> GetGrades(string entityId, List<string> itemId);

        /// <summary>
        /// Gets the grade list for a specific enrollemnt and itemid
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        DataContracts.GradeList GetGradeList(string enrollmentId, string itemId);

        /// <summary>
        /// Gets the history of submissions for an item in the gradebook for the specified user enrollment. 
        /// </summary>
        /// <param name="enrollmentId">Enrollment ID of user for which to get submission history.</param>
        /// <param name="itemId">ID of the item for which to get submission history.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetSubmissions(string enrollmentId, string itemId);

        /// <summary>
        /// Adds a student submission for an activity to the server.
        /// </summary>
        /// <param name="entityId">ID of the entity associated with the submission.</param>
        /// <param name="submission">The submission info to add to the server.</param>
        void AddStudentSubmission(string entityId, Submission submission);

        /// <summary>
        /// Gets a student's submission.
        /// </summary>
        /// <param name="enrollmentId">ID of the user's enrollment to which this student submission belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this student submission belongs.</param>
        /// <returns></returns>
        Submission GetStudentSubmission(string enrollmentId, string itemId);

        /// <summary>
        /// Gets a student's submission.
        /// </summary>
        /// <param name="enrollmentId">ID of the user's enrollment to which this student submission belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this student submission belongs.</param>
        /// <param name="version">Version of the teacher response to retrieve.</param>
        /// <returns></returns>
        Submission GetStudentSubmission(string enrollmentId, string itemId, int version);

        /// <summary>
        /// Retrieves information about one or more student submissions from the server. 
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentId">ID of the user’s enrollment to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetStudentSubmissionInfo(string entityId, string itemId, string enrollmentId, IEnrollmentActions enrollmentActions);

        /// <summary>
        /// Retrieves information about one or more student submissions from the server. 
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentId">ID of the user’s enrollment to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetStudentSubmissionInfo(string entityId, List<string> itemId, string enrollmentId, IEnrollmentActions enrollmentActions);

        /// <summary>
        /// Retrieves information about one or more student submissions from the server. 
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetStudentsSubmissionInfo(string entityId, string itemId, IEnrollmentActions enrollmentActions);


        /// <summary>
        /// Retrieves information about one or more student submissions from the server. 
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetStudentsSubmissionInfo(string entityId, List<string> itemId, IEnrollmentActions enrollmentActions);

        /// <summary>
        /// Retrieves submission information for the specified submission collection.
        /// See http://gls.agilix.com/Docs/Command/PutTeacherResponse
        /// </summary>
        /// <param name="submissions">The submissions to get information for.</param>
        IEnumerable<Submission> GetStudentsSubmissionInfo(IEnumerable<Submission> submissions);

        /// <summary>
        /// Gets the students submission info without teacher response.
        /// </summary>
        /// <param name="submissions">The submissions.</param>
        /// <returns></returns>
        IEnumerable<Submission> GetStudentsSubmissionInfoWithoutTeacherResponse(IEnumerable<Submission> submissions);

        /// <summary>
        /// Adds teacher response data including scores, comments, and grade status flags to the server.
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <param name="teacherResponse">The teacher response business object.</param>
        void AddTeacherResponse(string studentEnrollmentId, string itemId, TeacherResponse teacherResponse);

        /// <summary>
        /// Gets a teacher's response to a student's submission. 
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <returns></returns>
        TeacherResponse GetTeacherResponse(string studentEnrollmentId, string itemId);

        /// <summary>
        /// Gets a teacher's response to a student's submission.
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <param name="version">Version of the teacher response to retrieve.</param>
        /// <returns></returns>
        TeacherResponse GetTeacherResponse(string studentEnrollmentId, string itemId, int version);

        /// <summary>
        /// For a specified enrollment, gets the percent of graded items out of all gradable items.
        /// </summary>
        /// <param name="e">The enrollment.</param>
        /// <returns></returns>
        double GetPercentGraded(Enrollment enrollment);

        /// <summary>
        /// Uploads a document submission.
        /// </summary>
        /// <param name="entityId">ID of the parent course or section.</param>
        /// <param name="itemId">ID of the item to attach the document to.</param>
        /// <param name="docTitle">Title of the document.</param>
        /// <param name="fileStream">Stream to the content of the document.</param>
        /// <param name="outputType">Output type for the document.</param>
        /// <param name="resourceMapActions">The IResourceMapActions implementation.</param>
        /// <returns></returns>
        string UploadDocument(string entityId, string itemId, string docTitle, Stream fileStream, DocumentOutputType outputType, IResourceMapActions resourceMapActions);

        /// <summary>
        /// Gets a submission aspose document stream.
        /// </summary>
        /// <param name="resourcePaths">The resource paths.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="fileName">Name of the file.</param>
        Stream GetDocumentsStreamFromResource(IEnumerable<string> resourcePaths, string entityId, DocumentOutputType outputType, out string fileName);

        /// <summary>
        /// Returns submission aspose document stream if one document requested.
        /// Returns submission aspose document zipped stream if  more than one document requested.
        /// </summary>
        /// <param name="entityId">ID of the course.</param>
        /// <param name="enrollmentIds">Enrollment ID list for the submissions.</param>
        /// <param name="itemId">ID of the item to which submission made by student.</param>
        /// <param name="outputType">Download file format.</param>
        /// <param name="fileName">Output file name.</param>
        Stream GetSubmissionsStream(string entityId, List<string> enrollmentIds, string itemId, DocumentOutputType outputType, out string fileName);

        /// <summary>
        /// Returns submission aspose document stream if one document requested.
        /// Returns submission aspose document zipped stream if  more than one document requested.
        /// </summary>
        /// <param name="entityId">ID of the course.</param>
        /// <param name="enrollmentIds">Enrollment ID list for the submissions.</param>
        /// <param name="itemId">ID of the item to which submission made by student.</param>
        /// <param name="outputType">Download file format.</param>
        Stream GetDropboxSubmissionsStream(string entityId, string enrollmentId, string itemId, DataContracts.DocumentOutputType outputType);

        /// <summary>
        /// Retrieves information about one or more student submissions from the server.
        /// </summary>
        /// <param name="entityId">ID of the course or section to which the submissions belong.</param>
        /// <param name="enrollmentId">ID of the user’s enrollment to which the submissions belong.</param>
        /// <param name="itemIds">ID of the items (in the course manifest).</param>
        IEnumerable<Submission> GetStudentSubmissionInfoList(string entityId, string enrollmentId, List<string> itemIds);

        /// <summary>
        /// Gets the item and category gradebook weights for the specified entity.
        /// See http://gls.agilix.com/Docs/Command/GetGradebookWeights.
        /// </summary>
        /// <param name="entityId">ID of the course or section to get weights for.</param>
        /// <returns></returns>
        GradeBookWeights GetGradeBookWeights(string entityId);

        /// <summary>
        /// Determines whether the specified item has submissions.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>
        ///   <c>true</c> if the specified item has submissions; otherwise, <c>false</c>.
        /// </returns>
        bool HasSubmissions(string itemId);



        /// <summary>
        /// Determines whether [has saved document] [the specified enrollment id].
        /// </summary>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if [has saved document] [the specified enrollment id]; otherwise, <c>false</c>.
        /// </returns>
        bool HasSavedDocument(string enrollmentId, string itemId, string path);

        /// <summary>
        /// Gets grades by Enrollment
        /// </summary>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns>
        /// List of Grades
        /// </returns>
        IEnumerable<DataContracts.Grade> GetGradesByEnrollment(string enrollmentId, List<string> itemIds);

        /// <summary>
        /// Gets teacher responses for itemids specified
        /// </summary>
        /// <param name="itemIds">List of Item Ids</param>
        void GetTeacherResponseInfo(List<string> itemIds);

        /// <summary>
        /// Gets student submissions for given entityid (course)
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="outstanding"></param>
        /// <returns></returns>
        IEnumerable<Submission> GetEntitySubmissions(string entityId, bool outstanding = true);
    }
}