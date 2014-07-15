using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Logging;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public static class QuestionParserProvider
    {
        private static readonly IList<IQuestionParser> Parsers = new List<IQuestionParser>();

        public static void AddParser(IQuestionParser parser)
        {
            Parsers.Add(parser);
        }

        public static IEnumerable<ParsedQuestion> Parse(string data)
        {
            try
            {
                var parser = Parsers.FirstOrDefault(p => p.Recognize());
                if (parser != null)
                {
                    return parser.Parse(data);
                }
                throw new FormatException(data);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(
                    string.Format("QuestionParserProvider.GetParser: no parser configured for"), ex);
                throw;
            }
        }

        public static void Clear()
        {
            Parsers.Clear();
        }
    }
}
