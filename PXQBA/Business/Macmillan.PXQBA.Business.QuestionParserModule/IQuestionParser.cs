using System.Collections.Generic;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    /// <summary>
    /// Represents parser of file into questions
    /// </summary>
    public interface IQuestionParser
    {
        /// <summary>
        /// Checks if file name is recognizable for parsing
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Check result</returns>
        bool Recognize(string fileName);

        /// <summary>
        /// Parses the file and creates result with questions parsed and errors if any
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="file">File data</param>
        /// <returns>Validation result</returns>
        ValidationResult Parse(string fileName, byte[] file);
    }
}
