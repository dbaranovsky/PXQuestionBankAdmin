using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents the list of operations available for parsed file
    /// </summary>
    public interface IParsedFileOperation
    {
        /// <summary>
        /// Adds parsed file with questions into database
        /// </summary>
        /// <param name="fileName">Name of the parsed file</param>
        /// <param name="questionsData">Parsed questions data</param>
        /// <param name="resourcesData">Resources like images if any</param>
        /// <returns>Added file id</returns>
        long AddParsedFile(string fileName, string questionsData, byte[] resourcesData);

        /// <summary>
        /// Sets a particular status for existing parsed file
        /// </summary>
        /// <param name="id">File id</param>
        /// <param name="status">Status to set</param>
        /// <returns>Id of the file</returns>
        long SetParsedFileStatus(long id, ParsedFileStatus status);

        /// <summary>
        /// Loads parsed file, parsed questions and their resources from database
        /// </summary>
        /// <param name="id">File id to load</param>
        /// <returns>Parsed file</returns>
        ParsedFile GetParsedFile(long id);
    }
}