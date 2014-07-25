using System;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    [Serializable]
    public class ParsedResource
    {
        public string Name { get; set; }
        public byte[] BinData { get; set; }

        public string FullPath { get; set; }
    }
}
