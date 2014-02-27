using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// QB Administrative actions against DLAP.
    /// </summary>
    public interface IQuestionAdminActions
    {
    	/// <summary>
    	/// Adds a note to the system.
    	/// </summary>
    	/// <param name="questionNote"> </param>
    	/// <returns></returns>
    	void AddQuestionNote(QuestionNote questionNote);

    	/// <summary>
    	/// Gets question notes for specified question.
    	/// </summary>
    	/// <param name="questionId"> </param>
    	/// <returns></returns>
    	IEnumerable<QuestionNote> GetQuestionNotes(string questionId);

    	/// <summary>
    	/// Adds a question log to the system.
    	/// </summary>
    	/// <param name="questionLog"> </param>
    	/// <returns></returns>
    	void AddQuestionLog(QuestionLog questionLog);

    	/// <summary>
    	/// Gets question logs/history for specified question.
    	/// </summary>
    	/// <param name="questionId"> </param>
    	/// <returns></returns>
    	IEnumerable<QuestionLog> GetQuestionLogs(string questionId);

		/// <summary>
		/// Get Questions For Selected Quizzes
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="selectedQuizzes"></param>
		/// <returns></returns>
		//List<Question> GetQuestionsForSelectedQuizzes(string entityId, List<string> selectedQuizzes);
        List<Question> GetQuestionsForSelectedQuizzes(string entityId, List<ContentItem> selectedQuizzes);

        /// <summary>
        /// Enrolls current RA user to the Product course
        /// </summary>
        /// <param name="instructorPermissionFlags">Rights flag for Instructor</param>
        /// <returns>Flag</returns>
        bool CreateAdminUserEnrollment(string instructorPermissionFlags);

        /// <summary>
        /// Enrolls Anonymous User to the Discipline course       
        /// </summary>
        /// <param name="instructorPermissionFlags">Rights flag for Instructor</param>
        /// <returns>Enrollment Id</returns>
        string GetAnonymousUserEnrollmentId(string instructorPermissionFlags, string DisciplineCourseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questionId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        List<Question> GetQuestionAllVersions(string entityId, string questionId, bool version);
        string FindChangesInQuestions(Question OldQuestion,Question NewQuestion);
    }
}
