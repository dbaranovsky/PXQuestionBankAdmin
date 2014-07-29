using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Logging;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    /// <summary>
    /// Provider of the question files parsers list
    /// </summary>
    public static class QuestionParserProvider
    {
        private static readonly IList<IQuestionParser> Parsers = new List<IQuestionParser>();

        /// <summary>
        /// Add parser to the list
        /// </summary>
        /// <param name="parser">Parser to add</param>
        public static void AddParser(IQuestionParser parser)
        {
            Parsers.Add(parser);
        }

        /// <summary>
        /// Parses question file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="file">File data</param>
        /// <returns>Parsing result</returns>
        public static ValidationResult Parse(string fileName, byte[] file)
        {
            try
            {
                var parser = Parsers.FirstOrDefault(p => p.Recognize(fileName));
                if (parser != null)
                {
                    return parser.Parse(fileName, file);
                }
                throw new FormatException("QuestionParserProvider.Parse: file parameter");
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(
                    string.Format("QuestionParserProvider.GetParser: no parser configured for"), ex);
                throw;
            }
        }

        /// <summary>
        /// Clears parsers list
        /// </summary>
        public static void Clear()
        {
            Parsers.Clear();
        }
    }
}
