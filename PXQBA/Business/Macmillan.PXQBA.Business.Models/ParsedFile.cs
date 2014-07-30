namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Parsed file
    /// </summary>
    public class ParsedFile
    {
        /// <summary>
        /// Parsed file id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Parsed file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Parsed questions data
        /// </summary>
        public string QuestionsData { get; set; }

        /// <summary>
        /// Resources that are used in parsed questions
        /// </summary>
        public byte[] ResourcesData { get; set; }
    }
}