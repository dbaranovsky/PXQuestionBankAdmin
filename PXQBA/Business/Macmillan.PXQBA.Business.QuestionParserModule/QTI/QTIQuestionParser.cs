using System;
using System.Collections.Generic;
using System.IO;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QTI
{
    public class QTIQuestionParser : QuestionParserBase
    {
        public override bool Recognize(string fileName)
        {
            return String.Equals(Path.GetExtension(fileName), EnumHelper.GetEnumDescription(QuestionFileType.QTI), StringComparison.CurrentCultureIgnoreCase);
        }

        public override ValidationResult Parse(string fileName, byte[] file)
        {
            throw new NotImplementedException();
        }
    }
}
