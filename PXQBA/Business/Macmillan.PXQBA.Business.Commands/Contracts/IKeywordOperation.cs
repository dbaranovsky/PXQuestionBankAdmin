using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents list of operations available for keywords
    /// </summary>
    public interface IKeywordOperation
    {
        /// <summary>
        /// Loads the list of manually added (while question editing) keywords from database
        /// in addition to the list from course xml
        /// </summary>
        /// <param name="courseId">Id of the course to load keywords for</param>
        /// <param name="fieldName">Name of the metadata field of keywords type</param>
        /// <returns>List of keywords</returns>
        IEnumerable<string> GetKeywordList(string courseId, string fieldName);

        /// <summary>
        /// Adds manually added (while question editing) list of keywords to the database
        /// </summary>
        /// <param name="courseId">Id of the course to save keywords for</param>
        /// <param name="fieldName">Name of the keywords metadata field</param>
        /// <param name="keywords">List of keywords to add</param>
        void AddKeywords(string courseId, string fieldName, IEnumerable<string> keywords);
    }
}