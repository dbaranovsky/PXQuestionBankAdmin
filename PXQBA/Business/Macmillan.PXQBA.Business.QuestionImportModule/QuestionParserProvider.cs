using System;
using System.Collections.Generic;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Logging;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public static class QuestionParserProvider
    {
        private static readonly Dictionary<QuestionFileType, IQuestionParser> Parsers = new Dictionary<QuestionFileType, IQuestionParser>();

        public static void AddParser(QuestionFileType fileType, IQuestionParser parser)
        {
            if (Parsers.ContainsKey(fileType))
            {
                Parsers[fileType] = parser;
                return;
            }
            Parsers.Add(fileType, parser);
        }

        public static IQuestionParser GetParser(QuestionFileType fileType)
        {
            try
            {
                return Parsers[fileType];
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(
                    string.Format("QuestionParserProvider.GetParser: no parser configured for {0}", fileType), ex);
                throw;
            }
        }

        public static void Clear()
        {
            Parsers.Clear();
        }
    }
}
