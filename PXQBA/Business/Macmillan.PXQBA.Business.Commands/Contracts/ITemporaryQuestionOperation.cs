using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Is used for "workaround" operations that need to be done while editing question: copy question to temp course, editing it there, copy edited question back
    /// </summary>
    public interface ITemporaryQuestionOperation
    {
        /// <summary>
        /// Copies question from real course to temp course to increase editing speed
        /// </summary>
        /// <param name="questionIdToCopy">Question id to copy</param>
        Question CopyQuestionToTemporaryCourse(string sourceProductCourseId, string questionIdToCopy, string version = null);

        /// <summary>
        /// Copies question from temp course to source course
        /// </summary>
        /// <param name="sourceProductCourseId">Source product course id</param>
        /// <param name="sourceQuestionId">Source question id</param>
        /// <returns>Copied question</returns>
        Question CopyQuestionToSourceCourse(string sourceProductCourseId, string sourceQuestionId);

        /// <summary>
        /// Create question in temp course (not created in real course), set sequence according to the questions in real course, generate new id (it will be used when copying back to real course)
        /// </summary>
        /// <param name="question">Question to create</param>
        /// <returns>Created question</returns>
        Question CreateQuestion(string productCourseId, Question question);

      
    }
}