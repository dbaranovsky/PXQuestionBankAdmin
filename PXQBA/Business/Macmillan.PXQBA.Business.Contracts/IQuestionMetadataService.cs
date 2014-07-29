using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Represents service that manages business logic with questions metadata fields and values
    /// </summary>
    public interface IQuestionMetadataService
    {
        /// <summary>
        /// Loads the list of available question metadata fields for particular course
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>List of metadata fields and values</returns>
        IList<QuestionMetaField> GetAvailableFields(Course course);

        /// <summary>
        /// Loads available values for a particular question metadata fields in a particular course
        /// </summary>
        /// <param name="course">Course</param>
        /// <param name="fieldsNames">Metadata field names</param>
        /// <returns>List of metadata field values</returns>
        IList<QuestionMetaField> GetDataForFields(Course course, IEnumerable<string> fieldsNames);

        /// <summary>
        /// Loads question card layout configured for a particular course
        /// </summary>
        /// <param name="course">Course</param>
        /// <returns>Question card layout</returns>
        string GetQuestionCardLayout(Course course);
    }

}
