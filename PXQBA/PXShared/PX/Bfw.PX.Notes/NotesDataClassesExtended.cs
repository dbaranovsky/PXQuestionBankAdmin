using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Xml.Linq;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Notes
{
    /// <summary>
    /// Data context for notes data classes
    /// </summary>
    public partial class NotesDataClassesDataContext
    {
        /// <summary>
        /// Gets the item highlight notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetItemNotes")]
        [ResultType(typeof(Biz.DataContracts.Highlight))]
        [ResultType(typeof(BizDC.Note))]
        public IMultipleResults GetItemHighlightNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())), reqXml);
            return (IMultipleResults)result.ReturnValue;
        }

        /// <summary>
        /// Gets the item general notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetItemNotes")]
        public ISingleResult<BizDC.Note> GetItemGeneralNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())), reqXml);
            return ((ISingleResult<BizDC.Note>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets the submission highlight notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetSubmissionNotes")]
        [ResultType(typeof(Biz.DataContracts.Highlight))]
        [ResultType(typeof(BizDC.Note))]
        public IMultipleResults GetSubmissionHighlightNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), reqXml);
            return (IMultipleResults)result.ReturnValue;
        }

        /// <summary>
        /// Gets the submission general notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetSubmissionNotes")]
        public ISingleResult<BizDC.Note> GetSubmissionGeneralNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), reqXml);
            return ((ISingleResult<BizDC.Note>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets the review highlight notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetReviewNotes")]
        [ResultType(typeof(Biz.DataContracts.Highlight))]
        [ResultType(typeof(BizDC.Note))]
        public IMultipleResults GetReviewHighlightNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), reqXml);
            return (IMultipleResults)result.ReturnValue;
        }

        /// <summary>
        /// Gets the review general notes.
        /// </summary>
        /// <param name="reqXml">The req XML.</param>
        /// <returns></returns>
        [Function(Name = "GetReviewNotes")]
        public ISingleResult<BizDC.Note> GetReviewGeneralNotes([Parameter(DbType = "Xml")] XElement reqXml)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), reqXml);
            return ((ISingleResult<BizDC.Note>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets all shared notes.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        [Function(Name = "dbo.GetAllSharedNotes")]
        public ISingleResult<BizDC.ShareNoteResult> GetAllSharedNotes([Parameter(DbType = "NVarChar(50)")] string studentId, [Parameter(DbType = "NVarChar(50)")] string courseId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), studentId, courseId);
            return ((ISingleResult<BizDC.ShareNoteResult>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets all note settings.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        [Function(Name = "dbo.GetNoteSettings")]
        public ISingleResult<BizDC.ShareNoteResult> GetAllNoteSettings([Parameter(DbType = "NVarChar(50)")] string studentId, [Parameter(DbType = "NVarChar(50)")] string courseId, [Parameter(DbType = "NVarChar(50)")] string itemId, [Parameter(DbType = "NVarChar(50)")] string reviewId, [Parameter(DbType = "NVarChar(50)")] string enrollmentId)
        {
            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), studentId, courseId, itemId, reviewId, enrollmentId);
            return ((ISingleResult<BizDC.ShareNoteResult>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets all notes by user.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        [Function(Name = "dbo.GetNotesByUser")]
        public ISingleResult<BizDC.Note> GetAllNotesByUser([Parameter(DbType = "Int")] int? highlightType, [Parameter(DbType = "VarChar(50)")] string userId, [Parameter(DbType = "VarChar(50)")] string courseId, [Parameter(DbType = "VarChar(50)")] string enrollmentId)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), highlightType, userId, courseId, enrollmentId);
            return ((ISingleResult<BizDC.Note>)(result.ReturnValue));
        }

        /// <summary>
        /// Gets the notes for peer review by user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="enrollmentIds">The enrollment ids.</param>
        /// <returns></returns>
        [Function(Name = "dbo.GetNotesForPeerReview")]
        public ISingleResult<BizDC.Note> GetNotesForPeerReviewByUser([Parameter(DbType = "VarChar(50)")] string userId, [Parameter(DbType = "VarChar(MAX)")] string enrollmentIds)
        {
            var result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())), userId, enrollmentIds);
            return ((ISingleResult<BizDC.Note>)(result.ReturnValue));
        }

    }
}