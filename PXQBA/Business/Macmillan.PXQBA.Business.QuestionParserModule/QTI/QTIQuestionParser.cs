using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule.QTI
{
    public class QTIQuestionParser : QuestionParserBase
    {
        public override bool Recognize(string fileName)
        {
            if (Path.GetExtension(fileName).ToUpper() == "." + QuestionFileType.QTI)
            {
                return true;
            }
            return false;
        }

        public override ValidationResult Parse(string fileName, byte[] file)
        {
            throw new NotImplementedException();
        }
    }
}
