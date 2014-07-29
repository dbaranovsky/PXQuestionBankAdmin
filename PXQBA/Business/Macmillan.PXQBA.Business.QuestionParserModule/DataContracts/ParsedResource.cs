using System;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    /// <summary>
    /// Parsed resources that questions have reference to
    /// </summary>
    [Serializable]
    public class ParsedResource
    {
        /// <summary>
        /// Resource name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Binary data of resource
        /// </summary>
        public byte[] BinData { get; set; }

        /// <summary>
        /// Full path to the resource
        /// </summary>
        public string FullPath { get; set; }
    }
}
